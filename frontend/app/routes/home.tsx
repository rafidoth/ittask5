import { useQuery, useQueryClient } from "@tanstack/react-query";
import type { Route } from "./+types/home";
import type { GenerationResponse, Song } from "~/songs/types";
import SongsTable from "~/songs/SongsTable";
import Loading from "~/Loading";
import { getSongs } from "~/api";
import { Container, LoadingOverlay } from "@mantine/core";
import { useState, useEffect } from "react";
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
  const [page, setPage] = useState(1);

  useEffect(() => {
    setPage(1);
  }, [language, likes, seed]);

  const queryClient = useQueryClient();

  const { data, isPending, isFetching, isSuccess } = useQuery<GenerationResponse>({
    queryKey: ["songs", seed, language, likes, page],
    queryFn: () => getSongs(seed, language, likes, page),
    retry: false,
    staleTime: 60000,
  });

  useEffect(() => {
    if (isSuccess) {
      if (page < 1000) {
        queryClient.prefetchQuery({
          queryKey: ["songs", seed, language, likes, page + 1],
          queryFn: () => getSongs(seed, language, likes, page + 1),
          staleTime: 60000,
        });
      }

      if (page > 1) {
        queryClient.prefetchQuery({
          queryKey: ["songs", seed, language, likes, page - 1],
          queryFn: () => getSongs(seed, language, likes, page - 1),
          staleTime: 60000,
        });
      }
    }
  }, [page, isSuccess, seed, language, likes, queryClient]);

  const { songs } = data || {};
  return (
    <Container strategy="grid">
      <ParameterToolbar />
      <SongsTable
        data={songs || []}
        page={page}
        onPageChange={setPage}
        isLoading={isPending || isFetching}
      />
    </Container>)
};