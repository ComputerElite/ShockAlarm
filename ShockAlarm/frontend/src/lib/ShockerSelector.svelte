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
</style>
<div class="shockerContainer">
    {#each shockers as shocker}
        <div class="shocker">
            <p class="flex-middle">
                <label class="switch" style="margin-right: 10px;">
                    <input type="checkbox" bind:checked={shocker.Enabled}>
                    <span class="slider"></span>
                </label><b>{shocker.Name}</b></p>

            {#if shocker.Enabled}
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
            {/if}
        </div>
    {:else}
        <button on:click={updateShockers}>Fetch all shockers associated with account</button>
    {/each}
</div>