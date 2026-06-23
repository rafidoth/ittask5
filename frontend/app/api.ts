import axios from "axios";
import type { GenerationResponse, Song } from "./songs/types";

const baseUrl = import.meta.env.VITE_BASE_URL;

export const getSongs = async (seed: number, language: string, likes: number): Promise<GenerationResponse> => {
    const response = await axios.get(`${baseUrl}/api/songs`, {
        params: { seed, language, likes }
    });
    return response.data;
};
