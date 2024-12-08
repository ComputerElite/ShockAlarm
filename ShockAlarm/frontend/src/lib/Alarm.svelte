<script>
import ShockerSelector from "$lib/ShockerSelector.svelte";
import CronGenerator from "$lib/CronGenerator.svelte";
import { fetchJson } from "./Script.svelte";
import {onMount} from "svelte";

export let alarm = {
    Id: null,
    Cron: "0 0 12 ? * *",
    Enabled: false,
    Name: "New Alarm",
    Shockers: [],
    DisableAfterFirstTrigger: true,
    ToneId: null
}
export let tones = []

let initialEnabled = alarm.Enabled;

onMount(() => {
    initialEnabled = alarm.Enabled;
})

function save() {
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

function test() {
    fetchJson("/api/v1/alarms/test", {
        body: JSON.stringify(alarm),
        method: "POST"
    }, localStorage).then((res) => {
        if(!res.ok) {
            console.error("Failed to test alarm");
            alert(res.Error)
            return;
        }
        alert("Alarm triggered")
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

export let expanded = false;
function alarmChanged() {
    expanded = true;
}

$: if(initialEnabled != alarm.Enabled) {
    alarmChanged();
}
    
</script>
<style>
    .alarm {
        background: #222222;
        border: var(--accent-color) solid 1px;
        padding: 1em;
        border-radius: 0.5em;
    }
    
    .expand {
        background: none;
        border: none;
        border-radius: 0;
        transform: rotate(45deg);
        border-right: 2px solid var(--txt-color);
        border-bottom: 2px solid var(--txt-color);
        height: 10px;
        width: 10px;
        transition: transform 0.2s;
        margin: 10px;
    }
    
    .expanded {
        transform: rotate(225deg);
    }
</style>
<div class="shocker">
    <div class="between">
        <div class="flex-middle">
            <label class="switch">
                <input type="checkbox" bind:checked={alarm.Enabled}>
                <span class="slider"></span>
            </label>
            <input type="text" bind:value={alarm.Name}>
        </div>
        <button class={expanded ? "expand expanded" : "expand"} on:click={() => expanded = !expanded}></button>
    </div>
    <CronGenerator DisableAfterFirstTrigger={alarm.DisableAfterFirstTrigger} onChange={alarmChanged} bind:expanded={expanded} bind:timeZone={alarm.TimeZone} bind:cron={alarm.Cron}/>
    {#if expanded}
        <br>
        <h2>Shockers</h2>
        <ShockerSelector tones={tones} bind:shockers={alarm.Shockers}/>
        <button class="green" on:click={save}>{alarm.Id ? "Save" : "Add alarm"}</button>
        <button on:click={test}>Test alarm (trigger it)</button>
        {#if alarm.Id}
            <button class="red" on:click={del}>Delete</button>
        {/if}
    {/if}
</div>