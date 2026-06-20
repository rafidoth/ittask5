import { create } from "zustand";
import type { GenerationParameter, ViewMode } from "./types";


type ParameterStoreActions = {
    setLanguage: (language: string) => void;
    setLikes: (likes: number) => void;
    setSeed: (seed: number) => void;
    setViewMode: (mode: ViewMode) => void;
}


type ParameterStore = {
    parameters: GenerationParameter;
    viewMode: ViewMode;
    actions: ParameterStoreActions;
}


const useParameterToolbarStore = create<ParameterStore>((set) => ({
    parameters: {
        language: "en",
        likes: 3.7,
        seed: 425123,
    },
    viewMode: "table",
    actions: {
        setLanguage: (language: string) => set((state) => ({ parameters: { ...state.parameters, language } })),
        setLikes: (likes: number) => set((state) => ({ parameters: { ...state.parameters, likes } })),
        setSeed: (seed: number) => set((state) => ({ parameters: { ...state.parameters, seed } })),
        setViewMode: (mode: ViewMode) => set((state) => ({ viewMode: mode })),
    }
}));



export const useLanguage = () => useParameterToolbarStore((state) => state.parameters.language);
export const useLikes = () => useParameterToolbarStore((state) => state.parameters.likes);
export const useSeed = () => useParameterToolbarStore((state) => state.parameters.seed);
export const useViewMode = () => useParameterToolbarStore((state) => state.viewMode);
export const useParameterActions = () => useParameterToolbarStore((state) => state.actions);



