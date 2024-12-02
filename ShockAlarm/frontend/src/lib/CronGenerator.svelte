<script>
    import {onMount} from "svelte";

    export let cron = "0 0 12 ? * *";
    export let cronUtc = "0 0 12 ? * *";
    let second;
    let minute;
    let hour;
    let dayOfMonth;
    let month;
    let allWeekdays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    let weekdays = [];
    onMount(() => {
        updateBasedOnCron();
    });
    function updateBasedOnCron() {
        let parts = cron.split(" ");
        second = parts[0];
        minute = parts[1];
        hour = parts[2];
        dayOfMonth = parts[3] ?? "?";
        month = parts[4] ?? "*";
        for(let i = 0; i < allWeekdays.length; i++) {
            if(parts[5].includes(i)) {
                weekdays.push(i);
            }
        }
        allWeekdays = allWeekdays
    }
    
    function mapCron(array, dayChange = 0) {
        let d = weekdays.map(x => {
            let y = x + dayChange;
            if(y < 0) y = 6;
            if(y > 6) y = 0;
            return y;
        }).join(",")
        if(d == "") d = "*"
        return d;
    }
    
    function updateCron() {
        let weekdays 
        cron = `${second} ${minute} ${hour} ${dayOfMonth} ${month} ${mapCron(weekdays)}`;
        cronUtc = convertLocalTimeToUTCWithDayChange(second, minute, hour, dayOfMonth, month);
        cronUtc = `${cronUtc.utcSeconds} ${cronUtc.utcMinutes} ${cronUtc.utcHours} ${cronUtc.utcDayOfMonth} ${cronUtc.utcMonth} ${mapCron(weekdays, cronUtc.utcDayChange)}`;
    }

    function convertLocalTimeToUTCWithDayChange(seconds, minutes, hours, dayOfMonth, month) {
        // Split the localTime into hours and minutes
        // Get the current date
        const now = new Date();
        const wildcard = ["*", "?"];
        let usedDayOfMonth = wildcard.includes(dayOfMonth) ? now.getDay() : dayOfMonth;
        let usedMonth = wildcard.includes(month) ? now.getMonth() + 1 : month;

        // Create a new Date object with the local time
        const localDate = new Date(now.getFullYear(), usedMonth - 1, usedDayOfMonth, hours, minutes, seconds);

        // Save the original local day
        const localDay = localDate.getDay(); // 0 = Sunday, 1 = Monday, ..., 6 = Saturday

        // Convert to UTC time
        const utcDate = new Date(localDate.getTime() + localDate.getTimezoneOffset() * 60000);

        // Get the UTC day
        const utcDay = utcDate.getDay(); // 0 = Sunday, 1 = Monday, ..., 6 = Saturday

        // Check for day change
        let dayChange = utcDay - localDay;
        console.log(localDate.toString(), utcDate.toString())

        // Handle cases where the day wraps around (e.g., Saturday -> Sunday, Sunday -> Saturday)
        if (dayChange === -6) dayChange = 1; // Local Sunday to UTC Monday
        if (dayChange === 6) dayChange = -1; // Local Saturday to UTC Friday

        // Format the UTC time as hh:mm
        const utcMonth = wildcard.includes(month) ? month : utcDate.getUTCMonth() + 1;
        const utcDayOfMonth = wildcard.includes(dayOfMonth) ? dayOfMonth : utcDate.getUTCDate();
        const utcHours = utcDate.getUTCHours().toString();
        const utcMinutes = utcDate.getUTCMinutes().toString();
        const utcSeconds = utcDate.getUTCSeconds().toString();

        return {
            utcHours: utcHours,
            utcMinutes: utcMinutes,
            utcSeconds: utcSeconds,
            utcMonth: utcMonth,
            utcDayOfMonth: utcDayOfMonth,
            utcDayChange: dayChange, // -1 for previous day, 0 for same day, +1 for next day
        };
    }
    
    function toggleWeekday(day) {
        let weekdayIndex = allWeekdays.indexOf(day)
        let index = weekdays.indexOf(weekdayIndex);
        if(index === -1) {
            weekdays.push(weekdayIndex);
        } else {
            weekdays.splice(index, 1);
        }
        allWeekdays = allWeekdays
        updateCron()
    }
</script>
<style>
    code {
        background-color: #111111;
        padding: 0.5em;
        border-radius: 0.5em;
        margin-top: 25px;
    }
</style>
<div>
    {#each allWeekdays as day}
        <button class={weekdays.includes(allWeekdays.indexOf(day)) ? "round on" : "round off"} on:click={() => toggleWeekday(day)}>{day}</button>
    {/each}
</div>
<input type="number" min="0" max="23" step="1" on:change={updateCron} bind:value={hour}>:
<input type="number" min="0" max="59" step="1" on:change={updateCron} bind:value={minute}>:
<input type="number" min="0" max="59" step="1" on:change={updateCron} bind:value={second}>
<br>
<br>
<code>{cron}</code>
<code>{cronUtc}</code>