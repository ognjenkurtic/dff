const apiUrl = "http://localhost:5000";

const authForm = document.getElementById("auth_data");

function fetchApiKeyAndPrepareHeaders() {
    const apiKey = authForm.elements["api_key"].value;

    const requestHeaders = new Headers();
    requestHeaders.append('Content-Type', 'application/json');
    requestHeaders.append('X-Factor-API-Key', apiKey);

    return requestHeaders;
}