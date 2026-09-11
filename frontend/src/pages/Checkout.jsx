import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ShippingForm from "../components/ShippingForm";
import PaymentForm from "../components/PaymentForm";
import OrderSummary from "../components/OrderSummary";
import Spinner from "../components/Spinner";
import { getCartByUserId } from "../api/cartService";
import { createOrder } from "../api/orderService";
import { initiatePayment } from "../api/paymentService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";
import { useCart } from "../hooks/useCart";

function Checkout() {
  const { user } = useAuth();
  const { refreshCartCount } = useCart();
  const navigate = useNavigate();

  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState(null);

  const [step, setStep] = useState("shipping"); // 'shipping' | 'payment'
  const [shippingData, setShippingData] = useState(null);

  const [processing, setProcessing] = useState(false);
  const [submitError, setSubmitError] = useState(null);
  const [result, setResult] = useState(null); // { status: 'success' | 'failed', message }

  useEffect(() => {
    getCartByUserId(user.id)
      .then(setCart)
      .catch((err) => {
        if (err.response?.status === 404) {
          setCart({ cartItems: [] });
        } else {
          setLoadError(
            getErrorMessage(
              err,
              "Checkout is currently unavailable. Please try again soon.",
            ),
          );
        }
      })
      .finally(() => setLoading(false));
  }, [user.id]);

  const handleShippingSubmit = (data) => {
    setShippingData(data);
    setStep("payment");
  };

  const handlePayment = async () => {
    setProcessing(true);
    setSubmitError(null);
    try {
      const order = await createOrder({ userId: user.id, ...shippingData });
      const payment = await initiatePayment(order.id, order.totalAmount);

      if (payment.status === "Succeeded") {
        setResult({ status: "success" });
        refreshCartCount();
      } else {
        setResult({
          status: "failed",
          message:
            "Payment failed. Your order was not completed – feel free to try again.",
        });
      }
    } catch (err) {
      setSubmitError(
        getErrorMessage(
          err,
          "Checkout is currently unavailable. Please try again soon.",
        ),
      );
    } finally {
      setProcessing(false);
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Spinner text="Loading checkout..." />
      </div>
    );
  }

  if (loadError) {
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
          <p>{loadError}</p>
        </div>
      </div>
    );
  }

  const items = cart?.cartItems || [];

  if (items.length === 0 && !result) {
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
          <h2 style={{ marginTop: 0 }}>Your cart is empty</h2>
          <p>Add something lovely before checking out.</p>
          <button
            onClick={() => navigate("/home")}
            style={{ marginTop: "10px" }}
          >
            Back to Shop
          </button>
        </div>
      </div>
    );
  }

  if (processing) {
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
          <h2 style={{ marginTop: 0 }}>Processing Payment...</h2>
          <p>Please don't close this page.</p>
        </div>
      </div>
    );
  }

  if (result) {
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
          {result.status === "success" ? (
            <>
              <h2 style={{ marginTop: 0 }}>Thanks for your purchase! 🎉</h2>
              <p>Your order has been successfully placed and paid.</p>
              <button
                onClick={() => navigate("/home")}
                style={{ marginTop: "10px" }}
              >
                Continue Shopping
              </button>
            </>
          ) : (
            <>
              <h2 style={{ marginTop: 0 }}>Payment Failed</h2>
              <p>{result.message}</p>
              <button
                onClick={() => {
                  setResult(null);
                  setStep("shipping");
                }}
                style={{ marginTop: "10px" }}
              >
                Try Again
              </button>
            </>
          )}
        </div>
      </div>
    );
  }

  const total = items.reduce((sum, item) => sum + item.totalPrice, 0);

  return (
    <div style={{ minHeight: "100vh" }}>
      <div
        style={{
          padding: "2rem",
          maxWidth: "700px",
          margin: "0 auto",
          display: "flex",
          flexDirection: "column",
          gap: "20px",
        }}
      >
        <h1 style={{ textAlign: "center" }}>Checkout</h1>
        {submitError && (
          <p style={{ color: "#b33", textAlign: "center" }}>{submitError}</p>
        )}
        <OrderSummary items={items} total={total} />
        {step === "shipping" && (
          <ShippingForm onSubmit={handleShippingSubmit} submitting={false} />
        )}
        {step === "payment" && (
          <PaymentForm onSubmit={handlePayment} processing={processing} />
        )}
      </div>
    </div>
  );
}

export default Checkout;
