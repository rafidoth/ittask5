export type GenerationParameter = {
  language: string;
  likes: number;
  seed: number;
};


export type ViewMode = "table" | "grid";

export type Song = {
  id: string;
  title: string;
  artist: string;
  album: string;
  genre: string;
};

export type GenerationResponse = {
  success: boolean;
  songs: Song[];
};

