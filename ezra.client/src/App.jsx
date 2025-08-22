import { useEffect, useMemo, useState } from "react";
import "./App.css";

export default function App() {
    const [todos, setTodos] = useState([]);
    const [title, setTitle] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        load();
    }, []);

    async function load(retries = 2, delay = 300) {
        setLoading(true);
        try {
            let res = await fetch("/api/todos");
            while (!res.ok && retries-- > 0) {
                await new Promise((r) => setTimeout(r, delay));
                res = await fetch("/api/todos");
            }
            if (res.ok) setTodos(await res.json());
        } finally {
            setLoading(false);
        }
    }

    async function addTodo(e) {
        e.preventDefault();
        const t = title.trim();
        if (!t) return;
        const res = await fetch("/api/todos", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ title: t }),
        });
        if (res.ok) {
            setTitle("");
            await load();
        }
    }

    async function toggle(todo) {
        await fetch(`/api/todos/${todo.id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ isCompleted: !todo.isCompleted }),
        });
        await load();
    }

    async function remove(id) {
        await fetch(`/api/todos/${id}`, { method: "DELETE" });
        await load();
    }

    const incomplete = useMemo(() => todos.filter(t => !t.isCompleted), [todos]);
    const completed = useMemo(() => todos.filter(t => t.isCompleted), [todos]);

    return (
        <>
            {/* Step bar header */}
            <div className="header">
                <div className="steps" aria-label="Steps">
                    <span className="step active"><span className="dot" /> Add a task</span>
                    <span className="step"><span className="dot" /> Review</span>
                    <span className="step"><span className="dot" /> Done</span>
                </div>
            </div>

            <div className="container">
                {/* Card */}
                <div className="card" role="region" aria-labelledby="card-title">
                    <h1 id="card-title" className="h1">Add a task</h1>
                    <div className="sub">Please fill in the field below to add a new task.</div>

                    {/* Form “grid” like the screenshot */}
                    <form onSubmit={addTodo} className="form-grid" aria-label="Add task">
                        <div className="field full">
                            <label htmlFor="title" className="label">Task title</label>
                            <input
                                id="title"
                                className="input"
                                placeholder="e.g., Book a scan with Ezra"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                            />
                        </div>
                        {/* Actions (left Cancel, right Continue/Add) */}
                        <div className="actions">
                            <button type="button" className="btn secondary" onClick={() => setTitle("")}>
                                Cancel
                            </button>
                            <button type="submit" className="btn primary">
                                Continue
                            </button>
                        </div>
                    </form>
                </div>

                {/* Lists */}
                {loading ? (
                    <p className="empty">Loading…</p>
                ) : (
                    <>
                        <TodoSection
                            title="Incomplete"
                            items={incomplete}
                            toggle={toggle}
                            remove={remove}
                        />
                        <TodoSection
                            title="Completed"
                            items={completed}
                            toggle={toggle}
                            remove={remove}
                        />
                        {todos.length === 0 && <p className="empty">No tasks yet — add your first one above.</p>}
                    </>
                )}
            </div>
        </>
    );
}

function TodoSection({ title, items, toggle, remove }) {
    if (items.length === 0) return null;
    return (
        <section className="section" aria-label={title}>
            <h2>{title}</h2>
            <table className="table" aria-label={`${title} tasks`}>
                <thead>
                    <tr>
                        <th>Done</th>
                        <th>Title</th>
                        <th>Created</th>
                        <th />
                    </tr>
                </thead>
                <tbody>
                    {items.map((t) => (
                        <tr key={t.id}>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={t.isCompleted}
                                    onChange={() => toggle(t)}
                                    aria-label={`Toggle ${t.title}`}
                                />
                            </td>
                            <td className={t.isCompleted ? "row-strike" : ""}>{t.title}</td>
                            <td>{new Date(t.createdUtc).toLocaleString()}</td>
                            <td>
                                <button className="btn secondary" onClick={() => remove(t.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </section>
    );
}