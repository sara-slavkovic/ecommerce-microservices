function AuthFormCard({ title, children }) {
  return (
    <div
      style={{
        maxWidth: "400px",
        margin: "60px auto",
        padding: "30px",
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        textAlign: "center",
      }}
    >
      <h2>{title}</h2>
      {children}
    </div>
  );
}
export default AuthFormCard;
