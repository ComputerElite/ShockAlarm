<style>
    label {
        display: flex;
        align-items: center;
        justify-content: space-between;
    }
</style>
<CenteredBox>
    {#if state === "pwd"}
        <h1>Login</h1>
        <label for="username">Username<input bind:this={username} on:keydown={onKeyDownUsername} id="username" type="text" placeholder="Username" /></label><br>
        <label for="password">Password<input bind:this={password} on:keydown={onKeyDownPassword} id="password" type="password" placeholder="Password" /></label><br>
        <button on:click={startLogin}>Login</button>
        <button on:click={startRegister}>Register</button>
        {#if loginError}
            <ErrorBox>{loginError}</ErrorBox>
        {/if}
    {:else if state == "totp"}
        <h1>Two-factor authentication</h1>
        <label for="totp">TOTP<input id="totp" type="text" placeholder="TOTP" /></label><br>
        <button>Submit</button>
        <ErrorBox>2 FA is not implemented yet</ErrorBox>
    {/if}
</CenteredBox>

<script>
    import CenteredBox from "$lib/CenteredBox.svelte";
    import {fetchJson, hashStringSHA256, getRandomString, saveSession} from '$lib/Script.svelte';
    import ErrorBox from "$lib/ErrorBox.svelte";
    import {goto} from "$app/navigation";
    import { page } from '$app/stores';
    import {onMount} from "svelte";

    function onKeyDownUsername(e) {
        if(e.key == "Enter") password.focus()
    }
    function onKeyDownPassword(e) {
        if(e.key == "Enter") startLogin()
    }

    let afterLogin;
    
   
    onMount(() => {
        const sessionExpired = $page.url.searchParams.get('sessionExpired') == "true" || false;
        if(sessionExpired) loginError = "Session expired. Please log in agian"
        const customLoginError = $page.url.searchParams.get('loginError') || "";
        if(customLoginError) loginError = customLoginError;
        afterLogin = $page.url.searchParams.get('afterLogin') || localStorage.afterLogin;
        if(!afterLogin) afterLogin = "/"
    });

    let state = "pwd"
    let username;
    let password;
    let loginError = "";
    let twoFAChallengeId = "";
    function startRegister() {
        fetchJson("/api/v1/user/start_register", {
            method: "POST",
            body: JSON.stringify({
                Username: username.value
            })
        }).then((res) => {
            if(!res.Success) {
                // user doesn't exist or something like that
                loginError = res.Error;
                return;
            }
            console.log("server nonce is " + res.Nonce)
            const clientNonce = getRandomString();
            console.log("client nonce is " + clientNonce)
            // salted password hash to server
            hashStringSHA256(password.value + res.Nonce + clientNonce).then((passwordHash) => {
                fetchJson("/api/v1/user/register", {
                    method: "POST",
                    body: JSON.stringify({
                        Username: username.value,
                        PasswordHash: passwordHash,
                        CNonce: clientNonce,
                        ChallengeId: res.ChallengeId
                    })
                }).then((res) => {
                    if(!res.Success) {
                        // some error occurred, display it
                        loginError = res.Error;
                        return;
                    }
                    // user should be logged in, save session
                    saveSession(res.SessionId, localStorage)
                    goto(afterLogin)
                })
            })
            
        })
    }
    function startLogin() {
        fetchJson("/api/v1/user/start_login", {
            method: "POST",
            body: JSON.stringify({
                Username: username.value
            })
        }).then((res) => {
            if(!res.Success) {
                // user doesn't exist or something like that
                loginError = res.Error;
                return;
            }
            console.log("server nonce is " + res.Nonce)
            // salted password hash to server
            hashStringSHA256(password.value + res.Nonce).then((passwordHash) => {
                fetchJson("/api/v1/user/login", {
                    method: "POST",
                    body: JSON.stringify({
                        Username: username.value,
                        PasswordHash: passwordHash,
                        ChallengeId: res.ChallengeId
                    })
                }).then((res) => {
                    if(!res.Success) {
                        // some error occurred, display it
                        loginError = res.Error;
                        return;
                    }
                    if (res.Requires2fa) {
                        // userrequires 2fa, ask for TOTP token
                        twoFAChallengeId = res.ChallengeId
                        state = "totp"
                        return;
                    }
                    // user should be logged in, save session
                    saveSession(res.SessionId, localStorage)
                    goto(afterLogin)
                })
            })
            
        })

    }
</script>
