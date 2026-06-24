import { IconTable, IconLayoutGrid } from "@tabler/icons-react";
import { useViewMode, useParameterActions } from "./parametersStore";
import type { ViewMode } from "./types";

export function ViewToggle() {
    const viewMode = useViewMode();
    const setViewMode = useParameterActions().setViewMode;
    return (
        <div style={{
            display: "inline-flex",
            background: "var(--mantine-color-gray-1)",
            border: "1px solid var(--mantine-color-default-border)",
            borderRadius: "var(--mantine-radius-md)",
            padding: 3,
            gap: 2,
        }}>
            {[
                { value: "table" as ViewMode, icon: <IconTable size={18} /> },
                { value: "grid" as ViewMode, icon: <IconLayoutGrid size={18} /> },
            ].map(({ value, icon }) => (
                <button
                    key={value}
                    onClick={() => setViewMode(value)}
                    aria-pressed={viewMode === value}
                    style={{
                        width: 36,
                        height: 36,
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        border: "none",
                        borderRadius: "var(--mantine-radius-sm)",
                        cursor: "pointer",
                        background: viewMode === value ? "var(--mantine-primary-color-filled)" : "transparent",
                        color: viewMode === value ? "var(--mantine-color-white)" : "var(--mantine-color-dimmed)",
                        transition: "background 0.15s, color 0.15s",
                    }}
                >
                    {icon}
                </button>
            ))}
        </div>
    );
}