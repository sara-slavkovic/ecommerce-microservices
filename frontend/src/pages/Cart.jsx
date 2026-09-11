import { useEffect, useState } from "react";
import QuantityInput from "../components/QuantityInput";
import Spinner from "../components/Spinner";
import {
  getCartByUserId,
  removeCartItem,
  updateCartItemQuantity,
} from "../api/cartService";
import { getImageUrl } from "../api/catalogService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";
import { useNavigate } from "react-router-dom";
import { useToast } from "../hooks/useToast";
import { useCart } from "../hooks/useCart";
import { Link } from "react-router-dom";

function Cart() {
  const { user } = useAuth();
  const showToast = useToast();
  const { refreshCartCount } = useCart();
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    getCartByUserId(user.id)
      .then(setCart)
      .catch((err) => {
        if (err.response?.status === 404) {
          setCart({ cartItems: [] });
        } else {
          setError(
            getErrorMessage(
              err,
              "Cart is currently unavailable. Thanks for your patience!",
            ),
          );
        }
      })
      .finally(() => setLoading(false));
  }, [user.id]);

  const handleRemove = async (productId) => {
    try {
      await removeCartItem(user.id, productId);
      setCart((prev) => ({
        ...prev,
        cartItems: prev.cartItems.filter(
          (item) => item.productId !== productId,
        ),
      }));
      refreshCartCount();
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to remove item."), "error");
    }
  };

  const handleQuantityChange = async (item, newQuantity) => {
    try {
      await updateCartItemQuantity(user.id, item.productId, newQuantity);
      setCart((prev) => ({
        ...prev,
        cartItems: prev.cartItems.map((i) =>
          i.productId === item.productId
            ? {
                ...i,
                quantity: newQuantity,
                totalPrice: i.pricePerUnit * newQuantity,
              }
            : i,
        ),
      }));
      refreshCartCount();
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to update quantity."), "error");
    } finally {
      // Forces QuantityInput to remount and re-sync with the real (current) quantity, whether the update succeeded or failed.
      setRefreshKey((prev) => prev + 1);
    }
  };

  if (loading)
    return (
      <div style={{ minHeight: "100vh" }}>
        <Spinner text="Loading cart..." />
      </div>
    );
  if (error)
    return (
      <div style={{ minHeight: "100vh" }}>
        <div
          style={{
            maxWidth: "500px",
            margin: "80px auto",
            padding: "30px",
            backgroundColor: "var(--card-bg)",
            borderRadius: "8px",
            textAlign: "center",
          }}
        >
          <h2 style={{ marginTop: 0 }}>Oops!</h2>
          <p>{error}</p>
        </div>
      </div>
    );

  const items = cart?.cartItems || [];
  const total = items.reduce((sum, item) => sum + item.totalPrice, 0);

  return (
    <div style={{ minHeight: "100vh" }}>
      <div style={{ padding: "2rem", maxWidth: "800px", margin: "0 auto" }}>
        <h1
          style={{
            borderBottom: "1px solid var(--accent)",
            paddingBottom: "10px",
          }}
        >
          Your Shopping Cart
        </h1>

        {items.length === 0 ? (
          <p style={{ fontSize: "1.2rem", marginTop: "20px" }}>
            Your cart is beautifully empty.
          </p>
        ) : (
          <div>
            {items.map((item) => {
              const imageUrl = getImageUrl(item.productImageUrl);

              return (
                <div
                  key={item.id}
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    backgroundColor: "var(--card-bg)",
                    padding: "15px",
                    margin: "10px 0",
                    borderRadius: "5px",
                  }}
                >
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "1rem",
                    }}
                  >
                    <Link
                      to={`/product/${item.productId}`}
                      style={{
                        display: "flex",
                        alignItems: "center",
                        gap: "1rem",
                        textDecoration: "none",
                        color: "inherit",
                      }}
                    >
                      <img
                        src={imageUrl}
                        alt={item.productName}
                        style={{
                          width: "60px",
                          height: "60px",
                          objectFit: "cover",
                          borderRadius: "5px",
                        }}
                      />
                      <h3 style={{ margin: 0 }}>{item.productName}</h3>
                    </Link>
                    <QuantityInput
                      key={`${item.id}-${refreshKey}`}
                      initialQuantity={item.quantity}
                      onChange={(newQty) => handleQuantityChange(item, newQty)}
                    />
                  </div>
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "1rem",
                    }}
                  >
                    <h3 style={{ margin: 0 }}>${item.totalPrice.toFixed(2)}</h3>
                    <button
                      onClick={() => handleRemove(item.productId)}
                      style={{ padding: "6px 12px", fontSize: "0.85rem" }}
                    >
                      Remove
                    </button>
                  </div>
                </div>
              );
            })}

            <div style={{ textAlign: "right", marginTop: "20px" }}>
              <h2>Total: ${total.toFixed(2)}</h2>
              <button
                onClick={() => navigate("/checkout")}
                style={{
                  fontSize: "1.2rem",
                  padding: "15px 30px",
                  marginTop: "10px",
                }}
              >
                Proceed to Checkout
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default Cart;
