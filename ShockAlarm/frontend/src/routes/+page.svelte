<script>

    import { fetchJson } from "../lib/Script.svelte";
    import { goto } from "$app/navigation";
    import {onMount} from "svelte";
    import Alarm from "$lib/Alarm.svelte";
    import Token from "$lib/Token.svelte";
    
    onMount(() => {
        fetchJson("/api/v1/user/me", {}, localStorage).then((res) => {
            if(!res.ok) {
                localStorage.afterLogin = `/`
                goto("/login?loginError=Log in to manage your alarms and shockers");
                return;
            }
            updateTokens();
        });
    });
    
    let tokens = [];
    function updateTokens() {
        fetchJson("/api/v1/tokens", {}, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to fetch tokens");
                return;
            }
            tokens = res;
        });
        fetchAlarms();
    }
    let token;
    let alarms = [];
    function fetchAlarms() {
        fetchJson("/api/v1/alarms", {}, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to fetch alarms");
                return;
            }
            alarms = res;
        });
    }
    function addToken() {
        fetchJson("/api/v1/tokens", {
            body: JSON.stringify({
                Token: token
            }),
            method: "POST"
        }, localStorage).then((res) => {
            if(!res.ok) {
                alert(res.Error)
                return;
            }
            updateTokens();
        });
    }
    
</script>
{#if tokens.length > 0}
    <h1>Create alarm</h1>
    <Alarm/>
    <h1>Existing alarms</h1>
    {#each alarms as alarm}
        <Alarm alarm={alarm}/>
    {:else}
        <p>No alarms</p>
    {/each}
{:else}
    <p class="shocker">Add an access token from <a href="https://openshock.app/#/dashboard/tokens">OpenShock</a> below to create alarms for your shockers.</p>
{/if}
<h1>Add Token</h1>
<input bind:value={token} type="text" id="token" placeholder="Token">
<button on:click={addToken}>Add</button>
<h1>Current Tokens</h1>
{#each tokens as token}
    <Token token={token}/>
{:else}
    <p>No tokens</p>
{/each}