import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react";
import { getCartByUserId } from "../api/cartService";
import { useAuth } from "./useAuth";

const CartContext = createContext(null);

export function CartProvider({ children }) {
  const { user } = useAuth();
  const [itemCount, setItemCount] = useState(0);

  const refreshCartCount = useCallback(() => {
    if (!user) {
      setItemCount(0);
      return;
    }
    getCartByUserId(user.id)
      .then((cart) => {
        const count = (cart.cartItems || []).reduce(
          (sum, item) => sum + item.quantity,
          0,
        );
        setItemCount(count);
      })
      .catch(() => setItemCount(0)); // silent - badge just won't show a number
  }, [user]);

  useEffect(() => {
    refreshCartCount();
  }, [refreshCartCount]);

  return (
    <CartContext.Provider value={{ itemCount, refreshCartCount }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  return useContext(CartContext);
}
