import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getCampaigns } from '../services/api'

export default function CampaignList() {
  const [campaigns, setCampaigns] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const navigate = useNavigate()

  useEffect(() => {
    getCampaigns()
      .then(res => setCampaigns(res.data))
      .catch(() => setError('Failed to load campaigns'))
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <div>Loading campaigns...</div>
  if (error) return <div>{error}</div>

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Campaigns</h1>
        <button onClick={() => navigate('/campaigns/create')}>+ New Campaign</button>
      </div>

      {campaigns.length === 0 && <div>No campaigns found.</div>}

      {campaigns.map(campaign => (
        <div
          key={campaign.id}
          onClick={() => navigate(`/campaigns/${campaign.id}`)}
          style={{
            border: '1px solid #ccc',
            borderRadius: '8px',
            padding: '1rem',
            marginBottom: '1rem',
            cursor: 'pointer'
          }}
        >
          <div style={{ display: 'flex', justifyContent: 'space-between' }}>
            <h2 style={{ margin: 0 }}>{campaign.name}</h2>
            <span>{campaign.status}</span>
          </div>
          <div style={{ marginTop: '0.5rem', color: '#555' }}>
            Progress: {campaign.yesCount} / {campaign.goalTarget} confirmed
          </div>
          <div style={{ color: '#555' }}>
            Cooldown: {campaign.cooldownDays} days &nbsp;|&nbsp; Auto Close: {campaign.autoClose ? 'Yes' : 'No'}
          </div>
        </div>
      ))}
    </div>
  )
}