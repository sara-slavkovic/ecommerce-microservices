import {
  createContext,
  useContext,
  useState,
  useCallback,
  useRef,
} from "react";

const ConfirmContext = createContext(null);

export function ConfirmProvider({ children }) {
  const [dialog, setDialog] = useState(null);
  const resolveRef = useRef(null);

  const confirm = useCallback((message) => {
    setDialog(message);
    return new Promise((resolve) => {
      resolveRef.current = resolve;
    });
  }, []);

  const handleChoice = (result) => {
    setDialog(null);
    if (resolveRef.current) resolveRef.current(result);
  };

  return (
    <ConfirmContext.Provider value={confirm}>
      {children}
      {dialog && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: "rgba(0,0,0,0.4)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            zIndex: 2000,
          }}
        >
          <div
            style={{
              backgroundColor: "var(--bg-color)",
              borderRadius: "10px",
              padding: "30px",
              maxWidth: "360px",
              textAlign: "center",
              boxShadow: "0 10px 30px rgba(0,0,0,0.3)",
            }}
          >
            <p
              style={{
                margin: "0 0 25px 0",
                fontSize: "1.05rem",
                color: "var(--text-main)",
              }}
            >
              {dialog}
            </p>
            <div
              style={{ display: "flex", gap: "12px", justifyContent: "center" }}
            >
              <button
                onClick={() => handleChoice(false)}
                style={{
                  backgroundColor: "transparent",
                  color: "var(--text-main)",
                  border: "1px solid var(--text-main)",
                }}
              >
                Cancel
              </button>
              <button onClick={() => handleChoice(true)}>Delete</button>
            </div>
          </div>
        </div>
      )}
    </ConfirmContext.Provider>
  );
}

export function useConfirm() {
  return useContext(ConfirmContext);
}
