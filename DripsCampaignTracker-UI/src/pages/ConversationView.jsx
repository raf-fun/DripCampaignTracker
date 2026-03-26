import { useEffect, useState } from 'react'
import { useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { getMessages, sendMessage, getConversation } from '../services/api'

export default function ConversationView() {
  const { id } = useParams()
  const [searchParams] = useSearchParams()
  const leadName = searchParams.get('leadName') || `Conversation #${id}`
  const [messages, setMessages] = useState([])
  const [conversation, setConversation] = useState(null)
  const [input, setInput] = useState('')
  const [selectedSender, setSelectedSender] = useState('Marketer')
  const [loading, setLoading] = useState(true)
  const [sending, setSending] = useState(false)
  const [error, setError] = useState(null)
  const navigate = useNavigate()

  const fetchData = () => {
    Promise.all([getMessages(id), getConversation(id)])
      .then(([messagesRes, convRes]) => {
        setMessages(messagesRes.data)
        setConversation(convRes.data)
      })
      .catch(() => setError('Failed to load conversation'))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    fetchData()
  }, [id])

  const handleSend = async () => {
    if (!input.trim() || sending) return
    setSending(true)

    await sendMessage(id, {
      conversationId: parseInt(id),
      content: input,
      senderType: selectedSender === 'Marketer' ? 2 : 1
    })

    setInput('')
    fetchData()
    setSending(false)
  }

  const isCurrentSender = (senderType) => {
    if (selectedSender === 'Lead') return senderType === 'Lead'
    return senderType === 'Marketer' || senderType === 'AI'
  }

  const isBlocked = () => {
    if (!conversation) return false
    if (conversation.status === 'OptedOut') return true
    if (conversation.followUpCount >= 2 && conversation.status !== 'Completed') return true
    return false
  }

  const getBlockedMessage = () => {
    if (conversation?.status === 'OptedOut') return 'This lead has opted out and cannot be contacted again.'
    if (conversation?.followUpCount >= 2) return 'Maximum follow ups reached. This lead cannot be contacted again.'
    return null
  }

  const isCooldownActive = () => {
    if (!conversation) return false
    const daysSince = (Date.now() - new Date(conversation.lastContactedDate)) / (1000 * 60 * 60 * 24)
    return daysSince < conversation.cooldownDays
  }

  if (loading) return <div>Loading messages...</div>
  if (error) return <div>{error}</div>

  return (
    <div style={{ padding: '2rem' }}>
      <button onClick={() => navigate(-1)}>← Back</button>

      <h1 style={{ marginTop: '1rem' }}>{leadName}</h1>

      <div style={{ marginBottom: '1rem', color: '#aaa' }}>
        Status: {conversation?.status} &nbsp;|&nbsp; Follow ups: {conversation?.followUpCount} / 2
      </div>

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

      {isBlocked() && (
        <div style={{
          background: '#3a1a1a',
          border: '1px solid #aa3333',
          borderRadius: '8px',
          padding: '1rem',
          marginBottom: '1rem',
          color: '#ff6666'
        }}>
          {getBlockedMessage()}
        </div>
      )}

      {isCooldownActive() && !isBlocked() && (
        <div style={{
          background: '#2a2a1a',
          border: '1px solid #aaaa33',
          borderRadius: '8px',
          padding: '1rem',
          marginBottom: '1rem',
          color: '#ffff66'
        }}>
          Cooldown active — minimum {conversation.cooldownDays} days between messages.
        </div>
      )}

      {!isBlocked() && (
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
            disabled={isCooldownActive() && selectedSender === 'Marketer'}
            style={{ flex: 1, padding: '0.5rem' }}
          />
          <button
            onClick={handleSend}
            disabled={sending || (isCooldownActive() && selectedSender === 'Marketer')}
          >
            {sending ? 'Sending...' : 'Send'}
          </button>
        </div>
      )}

      {!isBlocked() && (
        <div style={{ fontSize: '0.75rem', color: '#aaa' }}>
          Select "Lead" to simulate an incoming reply and trigger AI classification
        </div>
      )}
    </div>
  )
}