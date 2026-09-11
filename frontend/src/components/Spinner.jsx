function Spinner({ text }) {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: "15px",
        padding: "2rem",
      }}
    >
      <div className="loader"></div>
      {text && <p style={{ margin: 0 }}>{text}</p>}
    </div>
  );
}

export default Spinner;
