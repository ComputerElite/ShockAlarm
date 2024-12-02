<script>
import ShockerSelector from "$lib/ShockerSelector.svelte";
import CronGenerator from "$lib/CronGenerator.svelte";
import { fetchJson } from "./Script.svelte";

export let alarm = {
    Id: null,
    Cron: "0 0 12 ? * *",
    Enabled: false,
    Name: "New Alarm",
    Shockers: [],
    DisableAfterFirstTrigger: false
}

function save() {
    console.log(alarm)
    fetchJson("/api/v1/alarms", {
        body: JSON.stringify(alarm),
        method: "POST"
    }, localStorage).then((res) => {
        if(!res.ok) {
            console.error("Failed to save alarm");
            alert(res.Error)
            return;
        }
        location.reload();
        alert("Alarm saved")
    })
}

function del() {
    fetchJson(`/api/v1/alarms`, {
        method: "DELETE",
        body: JSON.stringify({Id: alarm.Id})
    }, localStorage).then((res) => {
        if(!res.ok) {
            console.error("Failed to delete alarm");
            alert(res.Error)
            return;
        }
        alert("Alarm deleted")
        location.reload();
    })
}
</script>
<style>
    .alarm {
        background: #222222;
        border: var(--accent-color) solid 1px;
        padding: 1em;
        border-radius: 0.5em;
    }
</style>
<div class="alarm">
    <div class="flex-middle">
        <label class="switch">
            <input type="checkbox" bind:checked={alarm.Enabled}>
            <span class="slider"></span>
        </label>
        <input type="text" bind:value={alarm.Name}></div>
    
    <CronGenerator bind:timeZone={alarm.TimeZone} bind:cron={alarm.Cron}/>
    <br>
    <h2>Shockers</h2>
    <ShockerSelector bind:shockers={alarm.Shockers}/>
    <button on:click={save}>Save</button>
    <button on:click={del}>Delete</button>
</div>