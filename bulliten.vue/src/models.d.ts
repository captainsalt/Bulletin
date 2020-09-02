declare interface UserAccount {
  id: number;
  username: string;
}

declare interface Post {
  id: number;
  author: IUserAccount;
  content: string;
  creationDate: Date;
}
