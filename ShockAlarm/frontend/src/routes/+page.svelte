<script>

    import { fetchJson } from "../lib/Script.svelte";
    import { goto } from "$app/navigation";
    import {onMount} from "svelte";
    import Alarm from "$lib/Alarm.svelte";
    import Token from "$lib/Token.svelte";
    import {each} from "svelte/internal";
    import Tone from "$lib/Tone.svelte";
    
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
        fetchTones()
    }
    let token;
    let alarms = [];
    let tones = [];
    function updateShockers() {
        fetchJson("/api/v1/alarms?updateshockers=true", {}, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to fetch alarms");
                return;
            }
            alarms = res;
        });
    }
    function fetchAlarms() {
        fetchJson("/api/v1/alarms", {}, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to fetch alarms");
                return;
            }
            alarms = res;
        });
    }
    
    function fetchTones() {
        fetchJson("/api/v1/tones", {}, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to fetch tones");
                return;
            }
            tones = res;
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
    
    function alarmsUpdated() {
        alarmAdded = false;
        fetchAlarms();
    }
    
    function tonesUpdated() {
        toneAdded = false;
        fetchTones();
    }
    
    let alarmAdded = false;
    let toneAdded = false;
</script>
{#if tokens.length > 0}
    <button on:click={updateShockers}>Update shocker limits</button>
    <h1>Alarms</h1>
    {#if alarmAdded}
        <Alarm savedCallback={alarmsUpdated} tones={tones} expanded={true}/>
    {:else}
        <button class="round" style="font-size: 1.5rem;" on:click={() => alarmAdded = true}>+</button>
    {/if}
    {#each alarms as alarm}
        <Alarm savedCallback={alarmsUpdated} tones={tones} alarm={alarm}/>
    {:else}
        <p>No alarms</p>
    {/each}
{:else}
    <p class="shocker warning">Add an access token from <a href="https://openshock.app/#/dashboard/tokens">OpenShock</a> below to create alarms for your shockers.</p>
{/if}

<h1>Alarm Tones</h1>
{#if toneAdded}
    <Tone expanded={true} savedCallback={tonesUpdated}/>
{:else}
    <button class="round" style="font-size: 1.5rem;" on:click={() => toneAdded = true}>+</button>
{/if}
{#each tones as tone}
    <Tone savedCallback={tonesUpdated} {tone}/>
{:else}
    <p>No tones</p>
{/each}
<h1>Add Token</h1>
<input bind:value={token} type="text" id="token" placeholder="Token">
<button on:click={addToken}>Add</button>
<h1>Current Tokens</h1>
{#each tokens as token}
    <Token token={token}/>
{:else}
    <p>No tokens</p>
{/each}