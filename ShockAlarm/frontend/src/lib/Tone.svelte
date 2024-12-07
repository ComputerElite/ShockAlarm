<script>
import { fetchJson } from "./Script.svelte";
import {onMount} from "svelte";

export let tone = {
    Id: null,
    Name: "New alarm tone",
    Components: [],
    IsPublic: false
}

let initialEnabled = tone.Enabled;

onMount(() => {
    initialEnabled = tone.Enabled;
})

function save() {
    console.log(tone)
    fetchJson("/api/v1/tones", {
        body: JSON.stringify(tone),
        method: "POST"
    }, localStorage).then((res) => {
        if(!res.ok) {
            console.error("Failed to save alarm tone");
            alert(res.Error)
            return;
        }
        location.reload();
        alert("Alarm tone saved")
    })
}

function del() {
    fetchJson(`/api/v1/tones`, {
        method: "DELETE",
        body: JSON.stringify({Id: tone.Id})
    }, localStorage).then((res) => {
        if(!res.ok) {
            console.error("Failed to delete alarm");
            alert(res.Error)
            return;
        }
        alert("Alarm tone deleted")
        location.reload();
    })
}

export let expanded = false;
function toneChanged() {
    expanded = true;
}

$: if(initialEnabled != tone.Enabled) {
    toneChanged();
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
                <input type="checkbox" bind:checked={tone.Enabled}>
                <span class="slider"></span>
            </label>
            <input type="text" bind:value={tone.Name}>
        </div>
        <button class={expanded ? "expand expanded" : "expand"} on:click={() => expanded = !expanded}></button>
    </div>
    {#if expanded}
        <br>
        <h2>Components</h2>
        <ShockerSelector bind:shockers={tone.Shockers}/>
        <button class="green" on:click={save}>{tone.Id ? "Save" : "Add alarm"}</button>
        {#if tone.Id}
            <button class="red" on:click={del}>Delete</button>
        {/if}
    {/if}
</div>