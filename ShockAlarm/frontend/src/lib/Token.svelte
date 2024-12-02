<script>
    import { fetchJson } from "./Script.svelte";
    export let token;
    
    function del() {
        fetchJson(`/api/v1/tokens/`, {
            method: "DELETE",
            body: JSON.stringify({
                Id: token.Id
            })
        }, localStorage).then((res) => {
            if(!res.ok) {
                console.error("Failed to delete token");
                return;
            }
            location.reload();
        });
    }
                
</script>
<div class="shocker">
    <p>{token.Id}: {token.ForOpenShockUser}</p>
    <button on:click={del}>Delete</button>
</div>