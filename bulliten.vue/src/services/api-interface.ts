import axios from "axios";

const baseUrl: string = process.env.VUE_APP_API_URL;

export async function createAccount(username: string, password: string): Promise<Response> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  const response = await fetch(`${baseUrl}/api/user/create`, {
    method: "POST",
    mode: "cors",
    body: form
  });

  return response;
}
