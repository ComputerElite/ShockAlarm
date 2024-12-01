<script>
        export let active = false;
        export let toggle = false;
        export let onClose = () => {};
        
        $: if(toggle) {
            active = !active;
            toggle = false;
        }
</script>
<style>
    .fullscreen-popup {
        position: fixed;
        top: 0;
        left: 0;
        bottom: 0;
        right: 0;   
        background-color: #000000AA;
        z-index: 1000;
        padding: 10px;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .fullscreen-popup-container {
        position: relative;
        padding: 10px;
        width: 100%;
        height: 90%;
        background-color: var(--popup-bg-color);
        border-radius: var(--border-radius);
    }
</style>
{#if active}
    <div class="fullscreen-popup">
        <div class="fullscreen-popup-container">
            <button on:click={() => {
                active = false;
                onClose();
            }}>X</button>
            <slot></slot>
        </div>
    </div>
{/if}