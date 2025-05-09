export interface Comment {
  id: string;
  content: string;
  creatorUserName: string;
  createdAt: string;
}

export interface MyTopic {
  id: string;
  title: string;
  description: string;
  votes: number;
  presentationTimeInMinutes: number;
  expanded: boolean;
  comments: Comment[];
  isLoading: boolean;
}

export interface ForeignTopic {
  id: string;
  title: string;
  description: string;
  presenterUserName: string;
  presentationTimeInMinutes: number;
  didIVoteForThis: boolean;
  expanded: boolean;
  comments: Comment[];
  isLoading: boolean;
}