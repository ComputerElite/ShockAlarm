<script>
    import {onMount} from "svelte";

    export let cron = "0 0 12 ? * *";
    export let DisableAfterFirstTrigger;
    export let timeZone;
    export let expanded;
    export let onChange;
    let second;
    let minute;
    let hour;
    let dayOfMonth;
    let month;
    let allWeekdays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    let weekdays = [];
    onMount(() => {
        if(!timeZone) timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
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
        DisableAfterFirstTrigger = weekdays.length <= 0
        onChange();
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
<input type="text" bind:value={timeZone}>
{#if expanded}
    <br>
    <br>
    <code>{cron}</code>
{/if}