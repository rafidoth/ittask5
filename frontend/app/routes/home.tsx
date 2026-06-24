import type { Route } from "./+types/home";
import SongsTable from "~/songs/SongsTable";
import { Container } from "@mantine/core";
import ParameterToolbar from "~/songs/ParameterToolbar";
import { useViewMode } from "~/songs/parametersStore";
import SongsGrid from "~/songs/SongsGrid";
import Loading from "~/Loading";

export function meta({ }: Route.MetaArgs) {
  return [
    { title: "New React Router App" },
    { name: "description", content: "Welcome to React Router!" },
  ];
}

export default function Home() {
  const viewMode = useViewMode();
  return (
    <Container strategy="grid">
      <ParameterToolbar />
      {viewMode == "table" ? (
        <SongsTable />
      ) : <SongsGrid />}
    </Container>
  );
}