import { Table, ActionIcon, Text, Collapse, Paper } from '@mantine/core';
import { IconChevronDown } from '@tabler/icons-react';
import React from 'react';
import type { Song } from './types';
import SongDetails from './SongDetails';

type SongsTableRowProps = {
  song: Song;
  page: number;
  seed: number;
  isExpanded: boolean;
  onToggle: () => void;
};

export default function SongsTableRow({ song, page, seed, isExpanded, onToggle }: SongsTableRowProps) {
  return (
    <React.Fragment>
      <Table.Tr onClick={onToggle} style={{ cursor: 'pointer' }}>
        <Table.Td>
          <ActionIcon variant="transparent" c="gray.6">
            <IconChevronDown />
          </ActionIcon>
        </Table.Td>
        <Table.Td><Text fw={700}>{(page - 1) * 12 + song.id}</Text></Table.Td>
        <Table.Td>{song.title}</Table.Td>
        <Table.Td>{song.artist}</Table.Td>
        <Table.Td>{song.album == 'Single' ? <Text c="dimmed">Single</Text> : song.album}</Table.Td>
        <Table.Td>{song.genre}</Table.Td>
      </Table.Tr>

      <Table.Tr>
        <Table.Td colSpan={6} style={{ padding: 0, border: 0 }}>
          <Collapse expanded={isExpanded} transitionDuration={300} keepMounted={false}>
            <Paper style={{ width: "100%" }}>
              <SongDetails song={song} seed={seed} />
            </Paper>
          </Collapse>
        </Table.Td>
      </Table.Tr>
    </React.Fragment>
  );
}
