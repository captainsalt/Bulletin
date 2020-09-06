declare interface UserAccount {
  id: number;
  username: string;
  creationDate: string;
}

declare interface Post {
  id: number;
  author: IUserAccount;
  content: string;
  creationDate: string;
}

declare interface AuthResponse {
  token: string;
  user: UserAccount;
}
