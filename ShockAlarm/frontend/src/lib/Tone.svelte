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

function sortComponents() {
    tone.Components = tone.Components.sort((a, b) => a.TriggerSeconds - b.TriggerSeconds)
    tone.Components = tone.Components
}

function addComponent() {
    tone.Components.push({TriggerSeconds: 0, ControlType: 0, Intensity: 0, Duration: 0})
    sortComponents()
}

function deleteComponent(i) {
    tone.Components.splice(i, 1)
    sortComponents()
}

function cloneComponent(i) {
    tone.Components.push(JSON.parse(JSON.stringify(tone.Components[i])))
    sortComponents()
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
            <input type="text" bind:value={tone.Name}>
        </div>
        <button class={expanded ? "expand expanded" : "expand"} on:click={() => expanded = !expanded}></button>
    </div>
    {#if expanded}
        <br>
        <div class="flex-middle">
            <label class="switch">
                <input type="checkbox" bind:checked={tone.IsPublic}>
                <span class="slider"></span>
            </label>
            <label for="isPublic" style="margin-left: 20px;">Public</label>
        </div>
        <h2>Components</h2>
        {#each tone.Components as component, i}
            <div class="shockerContainer">
                <div class="shocker">
                    <div class="flex-middle">
                        <input type="number" style="width: 3m;" bind:value={component.TriggerSeconds} min="0" step="0.1">
                        <label for="triggerSeconds">Trigger seconds after alarm start</label>
                        <button class="red" on:click={() => deleteComponent(i)}>Delete</button>
                        <button on:click={() => cloneComponent(i)}>Clone</button>
                    </div>
                    <select bind:value={component.ControlType}>
                        <option value={0}>Stop</option>
                        <option value={1}>Shock</option>
                        <option value={2}>Vibrate</option>
                        <option value={3}>Sound</option>
                    </select>
                    <input type="number" bind:value={component.Intensity} min="0" max={100} step="1">
                    <label for="intensity">Intensity %</label>
                    <input type="number" bind:value={component.Duration} min="0" max={30000} step="1">
                    <label for="duration">Duration ms</label>
                </div>
            </div>
        {/each}
        <button on:click={addComponent}>Add component</button>
        <button class="green" on:click={save}>{tone.Id ? "Save" : "Add alarm tone"}</button>
        {#if tone.Id}
            <button class="red" on:click={del}>Delete</button>
        {/if}
    {/if}
</div>