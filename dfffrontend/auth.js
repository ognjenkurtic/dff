const apiUrl = "http://localhost:5000";

const authForm = document.getElementById("auth_data");

function fetchApiKeyAndPrepareHeaders() {
  let apiKey = localStorage.getItem("api_key");

  if (!apiKey) {
    apiKey = authForm.elements["api_key"].value;
    localStorage.setItem("api_key", apiKey);
  }

  const requestHeaders = new Headers();
  requestHeaders.append("Content-Type", "application/json");
  requestHeaders.append("X-Factor-API-Key", apiKey);

  return requestHeaders;
}
