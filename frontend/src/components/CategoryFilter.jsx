import { useState } from "react";

function CategoryFilter({ categories, selectedId, onSelect }) {
  const [expandedIds, setExpandedIds] = useState(new Set());

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
        style={{
          padding: "8px 10px",
          borderRadius: "5px",
          cursor: "pointer",
          fontWeight: selectedId === null ? "bold" : "normal",
          backgroundColor:
            selectedId === null ? "var(--accent)" : "transparent",
        }}
      >
        All Products
      </div>

      {topLevel.map((parent) => {
        const children = childrenOf(parent.id);
        const isExpanded = expandedIds.has(parent.id);

        return (
          <div key={parent.id}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                padding: "8px 10px",
                borderRadius: "5px",
                cursor: "pointer",
                fontWeight: selectedId === parent.id ? "bold" : "normal",
                backgroundColor:
                  selectedId === parent.id ? "var(--accent)" : "transparent",
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
                  style={{
                    padding: "6px 10px 6px 25px",
                    borderRadius: "5px",
                    cursor: "pointer",
                    fontSize: "0.9rem",
                    fontWeight: selectedId === child.id ? "bold" : "normal",
                    backgroundColor:
                      selectedId === child.id ? "var(--accent)" : "transparent",
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
