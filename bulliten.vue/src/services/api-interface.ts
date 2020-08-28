const baseUrl: string = process.env.VUE_APP_API_URL;

export function createAccount(username: string, password: string): Promise<Response> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  return fetch(`${baseUrl}/api/user/create`, {
    method: "POST",
    mode: "cors",
    body: form
  });
}

export function login(username: string, password: string): Promise<Response> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  return fetch(`${baseUrl}/api/user/login`, {
    method: "POST",
    mode: "cors",
    body: form
  });
}
