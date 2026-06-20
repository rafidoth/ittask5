import { Box, Paper, Select, Flex, Text, RollingNumber, Slider, Stack, SegmentedControl } from "@mantine/core";
import { IconArrowsShuffle, IconChevronDown, IconGrid4x4, IconLayoutGrid, IconTable } from "@tabler/icons-react";
import { useState } from "react";
import { useLanguage, useLikes, useParameterActions, useSeed, useViewMode } from "./parametersStore";
import type { ViewMode } from "./types";

export default function ParameterToolbar() {
    const language = useLanguage();
    const likes = useLikes();
    const seed = useSeed();
    const viewMode = useViewMode();
    const actions = useParameterActions();

    return (
        <Paper >
            <Flex justify={"space-between"} bg="gray.0" bdrs={"md"} align={"center"} p="md">
                <Flex p="md" gap="md">
                    <Box style={{ background: "white" }} bd={"1px solid gray.3"} px="md" py="xs" bdrs={"sm"} w={200}>
                        <Text p={0} m={0} size="xs" c="dimmed" fw={"bold"}>Language</Text>
                        <Select
                            rightSection={
                                <IconChevronDown size={18}
                                    style={{ cursor: "pointer", color: "black" }}
                                    onClick={() => console.log("Shuffle languages")}
                                />}
                            leftSectionProps={{ style: { padding: "0px" } }}
                            variant="unstyled"
                            data={[
                                { value: "en", label: "English (US)" },
                                { value: "es", label: "Spanish" },
                            ]}
                            value={language}
                            onChange={(value) => actions.setLanguage(value || "en")}
                        />
                    </Box>

                    <Box style={{ background: "white" }} bd={"1px solid gray.3"} px="md" py="xs" bdrs={"sm"} w={200}>
                        <Text p={0} mb={4} size="xs" c="dimmed" fw={"bold"}>Seed</Text>
                        <Flex justify={"space-between"} align={"center"}>
                            <RollingNumber value={seed} />
                            <IconArrowsShuffle
                                size={18} style={{ cursor: "pointer" }}
                                onClick={() => actions.setSeed(Math.floor(Math.random() * 10000))}
                            />
                        </Flex>
                    </Box>
                    <Stack justify="center" px="md" py="xs" bdrs={"sm"} w={200}>
                        <Text p={0} size="xs" c="dimmed" fw={"bold"}>Likes</Text>
                        <Slider
                            color="blue"
                            min={0}
                            max={10}
                            step={0.1}
                            value={likes}
                            marks={Array.from({ length: 11 }, (_, i) => ({ value: i }))}
                            onChange={(value) => actions.setLikes(value)}
                        />
                    </Stack>
                </Flex>
                <SegmentedControl
                    value={viewMode}
                    onChange={(value) => actions.setViewMode(value as ViewMode)}
                    color="blue"
                    withItemsBorders={true}
                    data={[
                        {
                            label: <div style={{ display: "flex", alignItems: "center", gap: 4 }}>
                                <IconTable />
                            </div>, value: 'view'
                        },
                        {
                            label: <div style={{ display: "flex", alignItems: "center", gap: 4 }}>
                                <IconLayoutGrid />
                            </div>, value: 'grid'
                        },

                    ]}
                />
            </Flex>
        </Paper>
    );
}