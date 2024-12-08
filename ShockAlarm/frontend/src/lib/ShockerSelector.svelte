<script>
    import {fetchJson} from "$lib/Script.svelte";
    import {onMount} from "svelte";

    export let shockers = [];
    export let tones = []
    
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
                </label><b>{shocker.Name}</b><span style="margin-left: 10px;">{shocker.Paused ? " (paused)" : ""}</span></p>

            {#if shocker.Enabled}
                <select bind:value={shocker.ToneId}>
                    <option value={null}>Custom</option>
                    {#each tones as tone}
                        <option value={tone.Id}>{tone.Name}</option>
                    {/each}
                </select>
                {#if !shocker.ToneId}
                    <select bind:value={shocker.ControlType}>
                        <option value={0}>Stop</option>
                        {#if shocker.Permissions.shock}
                            <option value={1}>Shock</option>
                        {/if}
                        {#if shocker.Permissions.vibrate}
                            <option value={2}>Vibrate</option>
                        {/if}
                        {#if shocker.Permissions.sound}
                            <option value={3}>Sound</option>
                        {/if}
                    </select>
                    <input type="number" bind:value={shocker.Intensity} min="0" max={shocker.Limits.intensity} step="1">
                    <label for="intensity">Intensity %</label>
                    <input type="number" bind:value={shocker.Duration} min="0" max={shocker.Limits.duration} step="1">
                    <label for="duration">Duration ms</label>
                {/if}
            {/if}
        </div>
    {:else}
        <button on:click={updateShockers}>Fetch all shockers associated with account</button>
    {/each}
</div>