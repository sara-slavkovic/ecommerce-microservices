import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import ProductCard from "../components/ProductCard";
import CategoryFilter from "../components/CategoryFilter";
import { getAllProducts } from "../api/catalogService";
import { getAllCategories } from "../api/catalogService";
import { addItemToCart } from "../api/cartService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";
import { useToast } from "../hooks/useToast";
import { useCart } from "../hooks/useCart";

function Home() {
  const { user } = useAuth();
  const showToast = useToast();
  const { refreshCartCount } = useCart();

  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([getAllProducts(), getAllCategories()])
      .then(([productsData, categoriesData]) => {
        setProducts(productsData);
        setCategories(categoriesData);
      })
      .catch((err) =>
        setError(
          getErrorMessage(
            err,
            "Products are currently unavailable. Thanks for your patience!",
          ),
        ),
      )
      .finally(() => setLoading(false));
  }, []);

  const handleAddToCart = async (product) => {
    try {
      await addItemToCart(user.id, product.id, 1);
      showToast(`${product.name} added to cart!`, "success");
      refreshCartCount();
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to add to cart."), "error");
    }
  };

  // A selected parent category should also show products from its subcategories
  const getMatchingCategoryIds = (categoryId) => {
    if (!categoryId) return null;
    const children = categories
      .filter((c) => c.parentCategoryId === categoryId)
      .map((c) => c.id);
    return [categoryId, ...children];
  };

  const matchingIds = getMatchingCategoryIds(selectedCategoryId);
  const filteredProducts = matchingIds
    ? products.filter((p) => matchingIds.includes(p.categoryId))
    : products;

  if (loading) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Navbar />
        <div style={{ padding: "4rem 2rem", textAlign: "center" }}>
          Loading products...
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Navbar />
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
  }

  return (
    <div style={{ minHeight: "100vh" }}>
      <Navbar />
      <div
        style={{
          padding: "2rem 3rem",
          display: "flex",
          gap: "2rem",
          alignItems: "flex-start",
        }}
      >
        <CategoryFilter
          categories={categories}
          selectedId={selectedCategoryId}
          onSelect={setSelectedCategoryId}
        />

        <div style={{ flex: 1, maxWidth: "1100px", margin: "0 auto" }}>
          <h1
            style={{
              textAlign: "center",
              fontSize: "2.5rem",
              marginBottom: "2rem",
            }}
          >
            Our Products
          </h1>

          {filteredProducts.length === 0 ? (
            <p style={{ textAlign: "center" }}>
              No products found in this category.
            </p>
          ) : (
            <div
              style={{
                display: "grid",
                gridTemplateColumns: "repeat(auto-fill, minmax(230px, 1fr))",
                gap: "20px",
              }}
            >
              {filteredProducts.map((p) => (
                <ProductCard
                  key={p.id}
                  product={p}
                  onAddToCart={handleAddToCart}
                />
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default Home;
