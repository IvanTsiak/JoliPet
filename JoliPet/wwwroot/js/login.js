function login() {
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    
    fetch('/api/Auth/login', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({ email: email, password: password })
    })
        .then((response) => {
            if (response.ok) {
                window.location.href = '/index.html';
            } else {
                document.getElementById('error-msg').style.display = 'block';
            }
        })
        .catch((error) => console.error('Error: ', error));
}