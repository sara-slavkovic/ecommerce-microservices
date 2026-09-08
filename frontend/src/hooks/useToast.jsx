import { createContext, useContext, useState, useCallback } from "react";

const ToastContext = createContext(null);

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);

  const showToast = useCallback((message, type = "info") => {
    const id = Date.now();
    setToasts((prev) => [...prev, { id, message, type }]);
    setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id));
    }, 3000);
  }, []);

  return (
    <ToastContext.Provider value={showToast}>
      {children}
      <div
        style={{
          position: "fixed",
          top: "20px",
          right: "20px",
          display: "flex",
          flexDirection: "column",
          gap: "10px",
          zIndex: 1000,
        }}
      >
        {toasts.map((toast) => (
          <div
            key={toast.id}
            style={{
              backgroundColor: "#ffffff",
              color: toast.type === "error" ? "#842029" : "var(--text-main)",
              borderLeft: `6px solid ${toast.type === "error" ? "#c0392b" : toast.type === "success" ? "#7a9d6f" : "var(--accent)"}`,
              padding: "18px 26px",
              borderRadius: "6px",
              boxShadow: "0 10px 28px rgba(0,0,0,0.3)",
              minWidth: "300px",
              maxWidth: "420px",
              fontWeight: 600,
              fontSize: "1.15rem",
            }}
          >
            {toast.message}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  return useContext(ToastContext);
}
