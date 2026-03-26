import { useEffect, useState } from 'react'
import { useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { getMessages, sendMessage } from '../services/api'

export default function ConversationView() {
  const { id } = useParams()
  const [messages, setMessages] = useState([])
  const [input, setInput] = useState('')
  const [selectedSender, setSelectedSender ] = useState('Marketer')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const navigate = useNavigate()

    const [searchParams] = useSearchParams()
    const leadName = searchParams.get('leadName') || `Conversation #${id}`

  const fetchMessages = () => {
    getMessages(id)
      .then(res => setMessages(res.data))
      .catch(() => setError('Failed to load messages'))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    fetchMessages()
  }, [id])

  const handleSend = async () => {
    if (!input.trim()) return

    await sendMessage(id, {
      conversationId: parseInt(id),
      content: input,
      senderType: selectedSender === 'Marketer' ? 2 : 1
    })

    setInput('')
    fetchMessages()
  }

  const isCurrentSender = (senderType) => {
  if (selectedSender === 'Lead') return senderType === 'Lead'
  return senderType === 'Marketer' || senderType === 'AI'
}

  if (loading) return <div>Loading messages...</div>
  if (error) return <div>{error}</div>

  return (
    <div style={{ padding: '2rem' }}>
      <button onClick={() => navigate(-1)}>← Back</button>

      <h1 style={{ marginTop: '1rem' }}>{leadName}</h1>

      <div style={{
        border: '1px solid #ccc',
        borderRadius: '8px',
        padding: '1rem',
        minHeight: '300px',
        marginBottom: '1rem',
        display: 'flex',
        flexDirection: 'column',
        gap: '0.75rem'
      }}>
        {messages.length === 0 && <div style={{ color: '#555' }}>No messages yet.</div>}
        {messages.map(msg => (
          <div
            key={msg.id}
            style={{
              alignSelf: isCurrentSender(msg.senderType) ? 'flex-end' : 'flex-start',
            background: msg.senderType === 'Lead' ? '#2a2a2a' : msg.senderType === 'AI' ? '#1a3a1a' : '#1a1a3a',
              padding: '0.5rem 1rem',
              borderRadius: '8px',
              maxWidth: '60%'
            }}
          >
            <div style={{ fontSize: '0.75rem', color: '#aaa', marginBottom: '0.25rem' }}>
              {msg.senderType} {msg.classification !== 'None' && `— ${msg.classification}`}
            </div>
            <div>{msg.content}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '0.5rem' }}>
        <select
          value={selectedSender}
          onChange={e => setSelectedSender(e.target.value)}
          style={{ padding: '0.5rem' }}
        >
          <option value="Marketer">Marketer</option>
          <option value="Lead">Lead</option>
        </select>
        <input
          value={input}
          onChange={e => setInput(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && handleSend()}
          placeholder="Type a message..."
          style={{ flex: 1, padding: '0.5rem' }}
        />
        <button onClick={handleSend}>Send</button>
      </div>
      <div style={{ fontSize: '0.75rem', color: '#aaa' }}>
        Select "Lead" to simulate an incoming reply and trigger AI classification
      </div>
    </div>
  )
}