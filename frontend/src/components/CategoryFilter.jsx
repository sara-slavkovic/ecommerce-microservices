import { useState } from "react";

function CategoryFilter({ categories, selectedId, onSelect }) {
  const [expandedIds, setExpandedIds] = useState(new Set());
  const [hoveredId, setHoveredId] = useState(null);

  const topLevel = categories.filter((c) => !c.parentCategoryId);
  const childrenOf = (parentId) =>
    categories.filter((c) => c.parentCategoryId === parentId);

  const toggleExpand = (id) => {
    setExpandedIds((prev) => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  };

  const rowStyle = (id, isSelected) => ({
    padding: "8px 10px",
    borderRadius: "5px",
    cursor: "pointer",
    fontWeight: isSelected ? "bold" : "normal",
    backgroundColor: isSelected
      ? "var(--accent)"
      : hoveredId === id
        ? "rgba(208, 184, 168, 0.4)"
        : "transparent",
    transition: "background-color 0.15s ease",
  });

  return (
    <div
      style={{
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        padding: "15px",
        minWidth: "220px",
      }}
    >
      <div
        onClick={() => onSelect(null)}
        onMouseEnter={() => setHoveredId("all")}
        onMouseLeave={() => setHoveredId(null)}
        style={rowStyle("all", selectedId === null)}
      >
        All Products
      </div>

      {topLevel.map((parent) => {
        const children = childrenOf(parent.id);
        const isExpanded = expandedIds.has(parent.id);

        return (
          <div key={parent.id}>
            <div
              onMouseEnter={() => setHoveredId(parent.id)}
              onMouseLeave={() => setHoveredId(null)}
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                ...rowStyle(parent.id, selectedId === parent.id),
              }}
            >
              <span onClick={() => onSelect(parent.id)} style={{ flex: 1 }}>
                {parent.name}
              </span>
              {children.length > 0 && (
                <span
                  onClick={() => toggleExpand(parent.id)}
                  style={{ padding: "0 4px" }}
                >
                  {isExpanded ? "▲" : "▼"}
                </span>
              )}
            </div>

            {isExpanded &&
              children.map((child) => (
                <div
                  key={child.id}
                  onClick={() => onSelect(child.id)}
                  onMouseEnter={() => setHoveredId(child.id)}
                  onMouseLeave={() => setHoveredId(null)}
                  style={{
                    ...rowStyle(child.id, selectedId === child.id),
                    padding: "6px 10px 6px 25px",
                    fontSize: "0.9rem",
                  }}
                >
                  {child.name}
                </div>
              ))}
          </div>
        );
      })}
    </div>
  );
}

export default CategoryFilter;
