import { create } from "zustand";
import type { GenerationParameter, ViewMode } from "./types";


type ParameterStoreActions = {
    setLanguage: (language: string) => void;
    setLikes: (likes: number) => void;
    setSeed: (seed: number) => void;
    setViewMode: (mode: ViewMode) => void;
    setCurrentlyPlayingId: (id: string | null) => void;
}


type ParameterStore = {
    parameters: GenerationParameter;
    viewMode: ViewMode;
    currentlyPlayingId: string | null;
    actions: ParameterStoreActions;
}


const useParameterToolbarStore = create<ParameterStore>((set) => ({
    parameters: {
        language: "en",
        likes: 3.7,
        seed: 425123,
    },
    viewMode: "table",
    currentlyPlayingId: null,
    actions: {
        setLanguage: (language: string) => set((state) => ({ parameters: { ...state.parameters, language } })),
        setLikes: (likes: number) => set((state) => ({ parameters: { ...state.parameters, likes } })),
        setSeed: (seed: number) => set((state) => ({ parameters: { ...state.parameters, seed } })),
        setViewMode: (mode: ViewMode) => set((state) => ({ viewMode: mode })),
        setCurrentlyPlayingId: (id: string | null) => set({ currentlyPlayingId: id }),
    }
}));



export const useLanguage = () => useParameterToolbarStore((state) => state.parameters.language);
export const useLikes = () => useParameterToolbarStore((state) => state.parameters.likes);
export const useSeed = () => useParameterToolbarStore((state) => state.parameters.seed);
export const useViewMode = () => useParameterToolbarStore((state) => state.viewMode);
export const useCurrentlyPlayingId = () => useParameterToolbarStore((state) => state.currentlyPlayingId);
export const useParameterActions = () => useParameterToolbarStore((state) => state.actions);

