document.addEventListener("DOMContentLoaded", () => {
    loadMyPet();
    loadTargets();
});

function loadMyPet() {
    fetch('/api/pets/my', {
        credentials: "include"
    })
    .then(response => {
        if (response.status === 401 || response.redirected) {
            window.location.href = '/login.html';
            throw new Error("Not authorized");
        }
        return response.json();
    })
        .then(pet => {
            document.getElementById("pet-name").innerHTML = pet.name;
            document.getElementById('pet-mood').innerText = pet.mood;
            document.getElementById('pet-level').innerText = pet.level;
            document.getElementById('pet-xp').innerText = pet.experience;
        })
        .catch(error => console.error("Error: ", error));
}

function loadTargets() {
    fetch('/api/Battles/targets', {})
    .then(response => {
        if (!response.ok) {
            throw new Error("Failed to load targets");
        }
        return response.json();
    })
        .then(targets => {
            const select = document.getElementById("defender-id");
            
            select.innerHTML = '<option value="">Choose your opponent...</option>';
            
            targets.forEach(target => {
                const option = document.createElement("option");
                option.value = target.id;
                option.innerText = `${target.name} (Owner: ${target.petOwner})`;
                
                select.appendChild(option);
            });
        })
    .catch(error => console.error("Error: ", error));
}
function attack() {
    const defenderId = document.getElementById("defender-id").value;
    
    if (!defenderId) {
        alert("Please select an opponent first!");
        return;
    }
    
    fetch(`/api/battles/${defenderId}/attack`, {
        method: 'POST',
    })
        .then(response => response.json())
        .then(result => {
            const resText = result.isWinner
                ? `You win! Damage deal: ${result.damageDealtOrTaken}`
                : `You lose! Damage deal: ${result.damageDealtOrTaken}`;
            document.getElementById('battle-result').innerText = resText;
            loadMyPet();
        })
    .catch(error => console.error("Error: ", error));
}

function talk() {
    const messageInput = document.getElementById("chat-message");
    const messageText = messageInput.value.trim();
    
    if (!messageText) {
        alert("Please enter a message");
        return;
    }
    
    fetch('/api/Interactions/talk', {
        'method': 'POST',
        'headers': {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ message: messageText })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to send message");
            }
            return response.json();
        })
        .then(result => {
            messageInput.value = "";
            
            document.getElementById('chat-result').innerText = 
                `Mood: ${result.moodChange}, XP: ${result.xpGained}`;
            
            loadMyPet();
        })
        .catch(error => console.error("Error: ", error));
}

function logout() {
    fetch(`/api/Auth/logout`, { method: 'POST' })
        .then(() => {
            window.location.href = '/login.html';
        })
}