import store from "@/store/index";
import router from "@/router/index";

const baseUrl: string = process.env.VUE_APP_API_URL;

function getAuthHeader(): Headers {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const token = (store.state as any).auth.token || "";
  const headers = new Headers();

  headers.append("Authorization", token);

  return headers;
}

async function fetchRequest<T>(method: string, route: string, options: RequestInit): Promise<T> {
  const response = await fetch(`${baseUrl}${route}`, {
    ...options,
    method,
    mode: "cors"
  });

  if (!response.ok) {
    if (response.status === 401) {
      router.push("/login");
      store.dispatch("logout");
    }

    throw new Error((await response.json()).message);
  }

  return response.json();
}

export async function getUsers(): Promise<UserAccount[]> {
  return fetchRequest("GET", "/user/all", {
    headers: getAuthHeader()
  });
}

export async function createAccount(username: string, password: string): Promise<AuthResponse> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  return fetchRequest("POST", "/user/create", {
    body: form
  });
}

export async function login(username: string, password: string): Promise<AuthResponse> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  return fetchRequest("POST", "/user/login", {
    body: form
  });
}

export async function createPost(content: string): Promise<void> {
  const form = new FormData();
  form.append("content", content);

  return fetchRequest("POST", "/post/create", {
    body: form,
    headers: getAuthHeader()
  });
}

export async function getPublicFeed(username: string): Promise<Post[]> {
  return fetchRequest("GET", `/post/feed/public?username=${username}`, {
    headers: getAuthHeader()
  });
}

export async function getPersonalFeed(): Promise<Post[]> {
  return fetchRequest("GET", "/post/feed/personal", {
    headers: getAuthHeader()
  });
}

export async function getUserProfile(username: string): Promise<UserProfile> {
  return fetchRequest("GET", `/user/profile?username=${username}`, {
    headers: getAuthHeader()
  });
}

export async function followUser(username: string): Promise<void> {
  await fetchRequest("POST", `/user/follow?username=${username}`, {
    headers: getAuthHeader()
  });
}

export async function unfollowUser(username: string): Promise<void> {
  await fetchRequest("DELETE", `/user/unfollow?username=${username}`, {
    headers: getAuthHeader()
  });
}

export async function likePost(postId: number): Promise<void> {
  await fetchRequest("POST", `/post/like?postId=${postId}`, {
    headers: getAuthHeader()
  });
}

export async function unlikePost(postId: number): Promise<void> {
  await fetchRequest("DELETE", `/post/like/remove?postId=${postId}`, {
    headers: getAuthHeader()
  });
}

export async function repost(postId: number): Promise<void> {
  await fetchRequest("POST", `/post/repost?postId=${postId}`, {
    headers: getAuthHeader()
  });
}

export async function unRepost(postId: number): Promise<void> {
  await fetchRequest("DELETE", `/post/repost/remove?postId=${postId}`, {
    headers: getAuthHeader()
  });
}

