import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { getCampaign } from '../services/api'

export default function CampaignDetail() {
  const { id } = useParams()
  const [campaign, setCampaign] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const navigate = useNavigate()

  useEffect(() => {
    getCampaign(id)
      .then(res => setCampaign(res.data))
      .catch(() => setError('Failed to load campaign'))
      .finally(() => setLoading(false))
  }, [id])

  if (loading) return <div>Loading campaign...</div>
  if (error) return <div>{error}</div>

  const progressPercent = Math.round((campaign.yesCount / campaign.goalTarget) * 100)

  return (
    <div style={{ padding: '2rem' }}>
      <button onClick={() => navigate('/')}>← Back</button>

      <div style={{ marginTop: '1rem', marginBottom: '1.5rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h1>{campaign.name}</h1>
          <span>{campaign.status}</span>
        </div>
        <div style={{ color: '#555' }}>
          Goal: {campaign.yesCount} / {campaign.goalTarget} confirmed ({progressPercent}%)
        </div>
        <div style={{ color: '#555' }}>
          Cooldown: {campaign.cooldownDays} days &nbsp;|&nbsp; Auto Close: {campaign.autoClose ? 'Yes' : 'No'}
        </div>
      </div>

      <h2>Conversations</h2>

      {campaign.conversations.length === 0 && <div>No conversations found.</div>}

      {campaign.conversations.map(conv => (
        <div
          key={conv.id}
          onClick={() => navigate(`/conversations/${conv.id}`)}
          style={{
            border: '1px solid #ccc',
            borderRadius: '8px',
            padding: '1rem',
            marginBottom: '1rem',
            cursor: 'pointer'
          }}
        >
          <div style={{ display: 'flex', justifyContent: 'space-between' }}>
            <span>Lead #{conv.leadId}</span>
            <span>{conv.status}</span>
          </div>
          <div style={{ color: '#555', marginTop: '0.25rem' }}>
            Follow ups used: {conv.followUpCount} / 2
          </div>
        </div>
      ))}
    </div>
  )
}