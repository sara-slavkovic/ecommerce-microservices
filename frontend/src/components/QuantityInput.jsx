import { useState } from "react";

function QuantityInput({ initialQuantity, onChange }) {
  const [quantity, setQuantity] = useState(String(initialQuantity));
  const [upHovered, setUpHovered] = useState(false);
  const [downHovered, setDownHovered] = useState(false);

  const handleTyping = (e) => {
    const value = e.target.value;
    if (/^\d*$/.test(value)) {
      setQuantity(value);
    }
  };

  const commit = (newQuantity) => {
    const safeValue = Math.max(1, newQuantity);
    setQuantity(String(safeValue));
    if (safeValue !== initialQuantity) {
      onChange(safeValue);
    }
  };

  const handleBlur = () => {
    const parsed = parseInt(quantity, 10);
    commit(isNaN(parsed) ? 1 : parsed);
  };

  const increment = () => commit((parseInt(quantity, 10) || 1) + 1);
  const decrement = () => commit((parseInt(quantity, 10) || 1) - 1);

  const arrowStyle = (hovered) => ({
    width: "20px",
    height: "13px",
    padding: 0,
    margin: 0,
    fontSize: "0.55rem",
    lineHeight: 1,
    backgroundColor: hovered ? "var(--text-main)" : "var(--bg-color)",
    color: hovered ? "var(--bg-color)" : "var(--text-main)",
    border: `1px solid ${hovered ? "var(--text-main)" : "var(--accent)"}`,
    cursor: "pointer",
    transition:
      "background-color 0.15s ease, color 0.15s ease, border-color 0.15s ease",
  });

  return (
    <div style={{ display: "flex", alignItems: "center", gap: "6px" }}>
      <input
        type="text"
        inputMode="numeric"
        value={quantity}
        onChange={handleTyping}
        onBlur={handleBlur}
        style={{
          width: "45px",
          textAlign: "center",
          padding: "5px",
          margin: 0,
        }}
      />
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "space-between",
          height: "26px",
        }}
      >
        <button
          onClick={increment}
          onMouseEnter={() => setUpHovered(true)}
          onMouseLeave={() => setUpHovered(false)}
          style={{
            ...arrowStyle(upHovered),
            borderTopLeftRadius: "4px",
            borderTopRightRadius: "4px",
            borderBottomLeftRadius: 0,
            borderBottomRightRadius: 0,
          }}
        >
          ▲
        </button>
        <button
          onClick={decrement}
          onMouseEnter={() => setDownHovered(true)}
          onMouseLeave={() => setDownHovered(false)}
          style={{
            ...arrowStyle(downHovered),
            borderTopLeftRadius: 0,
            borderTopRightRadius: 0,
            borderBottomLeftRadius: "4px",
            borderBottomRightRadius: "4px",
          }}
        >
          ▼
        </button>
      </div>
    </div>
  );
}

export default QuantityInput;
