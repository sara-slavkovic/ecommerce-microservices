import { useState } from "react";
import {
  getImageUrl,
  updateProduct,
  deleteProduct,
} from "../api/catalogService";
import { restockInventory } from "../api/inventoryService";
import { getErrorMessage } from "../api/errorHandling";
import { useToast } from "../hooks/useToast";
import { useConfirm } from "../hooks/useConfirm";

function AdminProductRow({ product, categories, onDeleted, onUpdated }) {
  const showToast = useToast();
  const confirm = useConfirm();
  const [restockAmount, setRestockAmount] = useState("");
  const [busy, setBusy] = useState(false);
  const [editing, setEditing] = useState(false);
  const [error, setError] = useState("");

  const [formData, setFormData] = useState({
    sku: product.sku,
    name: product.name,
    brand: product.brand,
    description: product.description,
    price: product.price,
    imageUrl: product.imageUrl,
    isActive: product.isActive,
    categoryId: product.categoryId,
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleDelete = async () => {
    const confirmed = await confirm(
      `Delete "${product.name}"? This cannot be undone.`,
    );
    if (!confirmed) return;

    setBusy(true);
    try {
      await deleteProduct(product.id);
      showToast(`${product.name} deleted.`, "success");
      onDeleted(product.id);
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to delete product."), "error");
    } finally {
      setBusy(false);
    }
  };

  const handleRestock = async () => {
    const qty = parseInt(restockAmount, 10);
    if (!qty || qty <= 0) return;
    setBusy(true);
    try {
      await restockInventory(product.id, qty);
      showToast(`Added ${qty} units to ${product.name}.`, "success");
      setRestockAmount("");
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to restock product."), "error");
    } finally {
      setBusy(false);
    }
  };

  const handleSaveEdit = async (e) => {
    e.preventDefault();
    setError("");
    setBusy(true);
    try {
      const dto = { ...formData, price: parseFloat(formData.price) };
      const updated = await updateProduct(product.id, dto);
      showToast(`${updated.name} updated.`, "success");
      onUpdated(updated);
      setEditing(false);
    } catch (err) {
      setError(getErrorMessage(err, "Failed to update product."));
    } finally {
      setBusy(false);
    }
  };

  if (editing) {
    return (
      <form
        onSubmit={handleSaveEdit}
        style={{
          backgroundColor: "var(--card-bg)",
          padding: "15px",
          borderRadius: "5px",
          marginBottom: "10px",
        }}
      >
        {error && <p style={{ color: "#b33" }}>{error}</p>}
        <input
          type="text"
          name="sku"
          placeholder="SKU"
          value={formData.sku}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="name"
          placeholder="Name"
          value={formData.name}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="brand"
          placeholder="Brand"
          value={formData.brand}
          onChange={handleChange}
          required
        />
        <textarea
          name="description"
          placeholder="Description"
          value={formData.description}
          onChange={handleChange}
          rows={3}
          required
        />
        <input
          type="number"
          step="0.01"
          name="price"
          placeholder="Price"
          value={formData.price}
          onChange={handleChange}
          required
        />
        <select
          name="categoryId"
          value={formData.categoryId}
          onChange={handleChange}
          required
          style={{
            width: "100%",
            padding: "10px",
            border: "1px solid var(--accent)",
            borderRadius: "5px",
            marginBottom: "15px",
          }}
        >
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
        <div style={{ display: "flex", gap: "10px" }}>
          <button type="submit" disabled={busy}>
            {busy ? "Saving..." : "Save"}
          </button>
          <button
            type="button"
            onClick={() => setEditing(false)}
            disabled={busy}
          >
            Cancel
          </button>
        </div>
      </form>
    );
  }

  return (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        gap: "1rem",
        backgroundColor: "var(--card-bg)",
        padding: "12px 15px",
        borderRadius: "5px",
        marginBottom: "10px",
      }}
    >
      <img
        src={getImageUrl(product.imageUrl)}
        alt={product.name}
        style={{
          width: "45px",
          height: "45px",
          objectFit: "cover",
          borderRadius: "5px",
        }}
      />
      <div style={{ flex: 1 }}>
        <p style={{ margin: 0 }}>{product.name}</p>
        <p style={{ margin: 0, fontSize: "0.85rem", fontStyle: "italic" }}>
          {product.brand} – ${product.price.toFixed(2)}
        </p>
      </div>

      <input
        type="number"
        placeholder="Qty"
        min="1"
        value={restockAmount}
        onChange={(e) => {
          const val = e.target.value;
          if (val === "" || /^\d+$/.test(val)) setRestockAmount(val);
        }}
        style={{ width: "70px", margin: 0 }}
        disabled={busy}
      />
      <button
        onClick={handleRestock}
        disabled={busy || !restockAmount}
        style={{ fontSize: "0.85rem", padding: "6px 12px" }}
      >
        Restock
      </button>
      <button
        onClick={() => setEditing(true)}
        disabled={busy}
        style={{ fontSize: "0.85rem", padding: "6px 12px" }}
      >
        Edit
      </button>
      <button
        onClick={handleDelete}
        disabled={busy}
        style={{ fontSize: "0.85rem", padding: "6px 12px" }}
      >
        Delete
      </button>
    </div>
  );
}

export default AdminProductRow;
