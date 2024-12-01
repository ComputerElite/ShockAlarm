<script>
    import {onMount} from "svelte";

    export let cron = "0 0 12 ? * *";
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
    
    function mapCron(array) {
        let d = weekdays.join(",")
        if(d == "") d = "*"
        return d;
    }
    
    function updateCron() {
        let weekdays 
        cron = `${second} ${minute} ${hour} ${dayOfMonth} ${month} ${mapCron(weekdays)}`;
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
        <input type="checkbox" id={day} on:change={() => toggleWeekday(day)} checked={weekdays.includes(allWeekdays.indexOf(day))}>
        <label for={day}>{day}</label>
    {/each}
</div>
<input type="number" min="0" max="23" step="1" on:change={updateCron} bind:value={hour}>:
<input type="number" min="0" max="59" step="1" on:change={updateCron} bind:value={minute}>:
<input type="number" min="0" max="59" step="1" on:change={updateCron} bind:value={second}>
<br>
<br>
<code>{cron}</code>