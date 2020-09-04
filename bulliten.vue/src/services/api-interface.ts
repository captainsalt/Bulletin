import store from "@/store/index";

const baseUrl: string = process.env.VUE_APP_API_URL;

function getAuthToken(): string {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return (store.state as any).auth.token;
}

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

export function createPost(content: string): Promise<Response> {
  const form = new FormData();
  const headers = new Headers();

  form.append("content", content);
  headers.append("Authorization", getAuthToken() || "");

  return fetch(`${baseUrl}/api/post/create`, {
    method: "POST",
    mode: "cors",
    body: form,
    headers
  });
}

export async function getPosts(): Promise<Post[]> {
  const headers = new Headers();
  headers.append("Authorization", getAuthToken() || "");

  const response = await fetch(`${baseUrl}/api/post`, {
    method: "GET",
    mode: "cors",
    headers
  });

  if (!response.ok)
    throw Error("Error getting posts");

  return (await response.json()).posts;
}

export async function getUser(username: string): Promise<UserAccount> {
  return {
    id: 1000,
    username: "Placeholder username"
  }
}
