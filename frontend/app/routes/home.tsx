import { useQuery } from "@tanstack/react-query";
import type { Route } from "./+types/home";
import type { GenerationResponse, Song } from "~/songs/types";
import SongsTable from "~/songs/SongsTable";
import { getSongs } from "~/api";
import { Container } from "@mantine/core";
import ParameterToolbar from "~/songs/ParameterToolbar";
import { useLanguage, useLikes, useSeed } from "~/songs/parametersStore";

export function meta({ }: Route.MetaArgs) {
  return [
    { title: "New React Router App" },
    { name: "description", content: "Welcome to React Router!" },
  ];
}

export default function Home() {
  const language = useLanguage();
  const likes = useLikes();
  const seed = useSeed();

  const { data, isPending } = useQuery<GenerationResponse>({
    queryKey: ["songs", seed, language, likes],
    queryFn: () => getSongs(seed, language, likes),
    retry: false,
  });

  if (isPending) {
    return <p>Loading songs...</p>;
  }
  const { songs } = data || {};
  return (
    <Container strategy="grid">
      <ParameterToolbar />
      <SongsTable data={songs || []} />
    </Container>)
};