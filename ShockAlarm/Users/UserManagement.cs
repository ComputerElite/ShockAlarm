using ShockAlarm.Database;

namespace ShockAlarm.Users;

public class UserManager
{
    public static List<Challenge> challenges = new List<Challenge>();
    public static TimeSpan SessionValidity = new TimeSpan(30, 0, 0, 0); // Sessions are valid for 30 days
    public UserManager()
    {
        
    }
    
    public static User? GetUserBySession(string session)
    {
        
        using(AppDbContext c = new ())
        {
            UserSession? s = c.Sessions.FirstOrDefault(x => x.Id == session);
            if(s == null)
            {
                return null;
            }
            // set last access and check if session is still valid
            s.LastAccess = DateTime.UtcNow;
            if (s.ValidUnti < s.LastAccess)
            {
                // remove session if it expired
                c.Sessions.Remove(s);
                c.SaveChanges();
                return null;
            }

            c.SaveChanges();
            return GetUserByUUID(s.UserId);
        }
    }
    public static User? GetUserByUUID(string id)
    {
        using(AppDbContext c = new())
        {
            User? u = c.Users.FirstOrDefault(x => x.Id == id);
            if(u != null)
            {
                return u;
            }
        }

        return null;
    }
    public static User? GetUserByUsername(string username)
    {
        using(AppDbContext c = new())
        {
            User? u = c.Users.FirstOrDefault(x => x.Username == username);
            if(u != null)
            {
                return u;
            }
        }

        return null;
    }

    public static LoginResponse InitiateRegister(string username)
    {
        if(username == "") {
            return new LoginResponse {Error = "Username must not be empty"};
        }
        User? u = GetUserByUsername(username);
        if(u != null)
        {
            return new LoginResponse { Error = "User already exists" };
        }
        Challenge challenge = new Challenge
        {
            Username = username,
            Nonce = CryptographicsHelper.GetRandomString(32, 32),
            Id = Guid.NewGuid().ToString(),
            Type = ChallengeType.Register
        };
        // remove previous login attempts for this user
        challenges.RemoveAll(x => x.Username == u.Username);
        // add current login attempt
        challenges.Add(challenge);
        return new LoginResponse()
        {
            Nonce = challenge.Nonce.ToString(),
            ChallengeId = challenge.Id,
            Success = true
        };
    }

    public static LoginResponse InitiateLogin(string username)
    {
        User? u = GetUserByUsername(username);
        if(u == null)
        {
            return new LoginResponse { Error = "Invalid username or password" };
        }
        Challenge challenge = new Challenge
        {
            UserId = u.Id,
            Nonce = u.Salt,
            Id = Guid.NewGuid().ToString(),
            Type = ChallengeType.Password
        };
        // remove previous login attempts for this user
        challenges.RemoveAll(x => x.UserId == u.Id);
        // add current login attempt
        challenges.Add(challenge);
        return new LoginResponse()
        {
            Nonce = challenge.Nonce.ToString(),
            ChallengeId = challenge.Id,
            Success = true
        };
    }

    public static UserSession CreateUserSession(User User, TimeSpan validFor)
    {
        return CreateUserSession(User, DateTime.UtcNow + validFor);
    }

    public static UserSession CreateUserSession(User User, DateTime validUntil)
    {
        UserSession session = new UserSession
        {
            UserId = User.Id,
            CreationDate = DateTime.UtcNow,
            LastAccess = DateTime.UtcNow,
            ValidUnti = validUntil,
            Origin= null,
            Id = CryptographicsHelper.GetRandomString(100, 100)
        };
        using (AppDbContext c = new())
        {
            c.Sessions.Add(session);
            c.SaveChanges();
        }
        return session;
    }

    public static LoginResponse Login(LoginRequest request)
    {
        User? u = GetUserByUsername(request.Username);
        if(u == null)
        {
            return new LoginResponse { Error = "User doesn't exist" };
        }
        
        // hash password
        string hash = CryptographicsHelper.GetHash(request.Password + u.Salt).ToLower();
        if (u.PasswordHash != hash)
        {
            return new LoginResponse { Error = "Password incorrect" };
        }
        
        if (u.TwoFactorEnabled)
        {
            Challenge newRl = new Challenge { Id = Guid.NewGuid().ToString(), UserId = u.Id , Type = ChallengeType.TOTP};
            challenges.Add(newRl);
            return new LoginResponse { Success = true, Requires2fa = true, ChallengeId = newRl.Id};
        }

        UserSession session = CreateUserSession(u, SessionValidity);

        return new LoginResponse()
        {
            Success = true,
            SessionId = session.Id
        };
    }

    public static List<UserSession> GetSessionsForUser(User User)
    {
        using(AppDbContext c = new())
        {
            return c.Sessions.Where(x => x.UserId == User.Id).ToList();
        }
    }

    public static LoginResponse Register(LoginRequest request)
    {
        User? u = GetUserByUsername(request.Username);
        if(u != null)
        {
            return new LoginResponse { Error = "User already exists" };
        }
        String salt = CryptographicsHelper.GetRandomString(64, 64);
        u = new User
        {
            Username = request.Username,
            PasswordHash = CryptographicsHelper.GetHash(request.Password + salt).ToLower(),
            Salt = salt,
            TwoFactorEnabled = false
        };
        using (AppDbContext c = new())
        {
            c.Users.Add(u);
            c.SaveChanges();
        }
        // create session
        UserSession s = CreateUserSession(u, SessionValidity);
        return new LoginResponse { Success = true, SessionId = s.Id };
    }
}