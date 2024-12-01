<script>
    import {fetchJson} from "$lib/Script.svelte";
    import {onMount} from "svelte";

    export let shockers = [];
    
    function updateShockers() {
        fetchJson(`/api/v1/shockers/`, {}, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to fetch shockers");
                return;
            }
            shockers = res
        });
    }
</script>
<style>
    .shocker {
        background: #222222;
        border: var(--accent-color) solid 1px;
        border-radius: var(--border-radius);
        padding: 1em;
        marin-bottom: 1em;
    }
    
    body {
        margin: 20px;
    }
</style>
<div class="shockerContainer">
    {#each shockers as shocker}
        <div class="shocker">
            <p><b>{shocker.Name}</b> ({shocker.ShockerId})</p>
            <input type="checkbox" bind:checked={shocker.Enabled}>
            <select bind:value={shocker.ControlType}>
                <option value={0}>Stop</option>
                <option value={1}>Shock</option>
                <option value={2}>Vibrate</option>
                <option value={3}>Sound</option>
            </select>
            <input type="number" bind:value={shocker.Intensity} min="0" max="100" step="1">
            <label for="intensity">Intensity %</label>
            <input type="number" bind:value={shocker.Duration} min="0" max="30000" step="1">
            <label for="duration">Duration ms</label>
        </div>
    {:else}
        <button on:click={updateShockers}>Fetch all shockers associated with account</button>
    {/each}
</div>