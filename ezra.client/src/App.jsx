import { useEffect, useState } from 'react';
import './App.css';

export default function App() {
    const [todos, setTodos] = useState([]);
    const [title, setTitle] = useState('');
    const [loading, setLoading] = useState(true);

    useEffect(() => { load(); }, []);

    async function load() {
        setLoading(true);
        const res = await fetch('/api/todos');
        if (res.ok) setTodos(await res.json());
        setLoading(false);
    }

    async function addTodo(e) {
        e.preventDefault();
        const t = title.trim();
        if (!t) return;
        const res = await fetch('/api/todos', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ title: t })
        });
        if (res.ok) {
            setTitle('');
            await load();
        }
    }

    async function toggle(todo) {
        await fetch(`/api/todos/${todo.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ isCompleted: !todo.isCompleted })
        });
        await load();
    }

    async function remove(id) {
        await fetch(`/api/todos/${id}`, { method: 'DELETE' });
        await load();
    }

    const incomplete = todos.filter(t => !t.isCompleted);
    const completed = todos.filter(t => t.isCompleted);

    return (
        <div>
            <h1>Todos</h1>

            <form onSubmit={addTodo} style={{ display: 'flex', gap: '0.5rem', justifyContent: 'center' }}>
                <input
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder="Add a task…"
                    aria-label="Todo title"
                />
                <button type="submit">Add</button>
            </form>

            {loading ? (
                <p><em>Loading…</em></p>
            ) : (
                <>
                    <TodoSection
                        title="Incomplete"
                        todos={incomplete}
                        toggle={toggle}
                        remove={remove}
                    />

                    <TodoSection
                        title="Completed"
                        todos={completed}
                        toggle={toggle}
                        remove={remove}
                    />
                </>
            )}
        </div>
    );
}

function TodoSection({ title, todos, toggle, remove }) {
    if (todos.length === 0) return null;

    return (
        <>
            <h2>{title}</h2>
            <table className="table table-striped" aria-label={`${title} todos`}>
                <thead>
                    <tr>
                        <th>Done</th>
                        <th>Title</th>
                        <th>Created (UTC)</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {todos.map(t => (
                        <tr key={t.id}>
                            <td>
                                <input type="checkbox" checked={t.isCompleted} onChange={() => toggle(t)} />
                            </td>
                            <td style={{ textDecoration: t.isCompleted ? 'line-through' : 'none' }}>{t.title}</td>
                            <td>{new Date(t.createdUtc).toLocaleString()}</td>
                            <td>
                                <button onClick={() => remove(t.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}
