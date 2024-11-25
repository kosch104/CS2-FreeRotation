using System.Reflection;
using Colossal.Entities;
using Colossal.Logging;
using Game;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;

namespace FreeRotation.Systems
{
    /// <summary>
    /// Overrides tree state on placement with object tool based on setting.
    /// </summary>
    public partial class RotationSystem : GameSystemBase
    {
        public static RotationSystem Instance;
        private EntityQuery m_ObjectDefinitionQuery;
        private ObjectToolSystem m_ObjectToolSystem;
        private ILog m_Log;
        private Unity.Mathematics.Random m_Random;
        private ToolSystem m_ToolSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationSystem"/> class.
        /// </summary>
        public RotationSystem()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.log;
            m_ObjectToolSystem = World.GetOrCreateSystemManaged<ObjectToolSystem>();
        }

        private void ForceUpdate()
        {
            var field = typeof(ObjectToolSystem).GetField("m_ForceUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(m_ObjectToolSystem, true);
            }
            else
            {
                m_Log.Error($"Field m_ForceUpdate not found in ObjectToolSystem.");
            }
        }


        public void OnFreelyRotateLeft(InputActionPhase phase)
        {
            m_Log.Info("Phase: " + phase);
            /*if (phase == InputActionPhase.Performed)
            {
                ForceUpdate();
                m_Log.Info($"Previous Variation: {m_RandomSeed}");
            }*/
        }

        public void OnFreelyRotateRight(InputActionPhase phase)
        {
            /*if (phase == InputActionPhase.Performed)
            {
                ForceUpdate();
                m_Log.Info($"Next Variation: {m_RandomSeed}");
            }*/
        }


        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            m_ObjectDefinitionQuery = SystemAPI.QueryBuilder()
                .WithAllRW<CreationDefinition>()
                .WithAll<Updated>()
                .WithNone<Deleted, Overridden>()
                .Build();

            RequireForUpdate(m_ObjectDefinitionQuery);
            NativeArray<Entity> entities = m_ObjectDefinitionQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (!EntityManager.TryGetComponent(entity, out ObjectDefinition currentObjectDefinition))
                {
                    entities.Dispose();
                    return;
                }

                currentObjectDefinition.m_Rotation = Quaternion.Euler(0, m_Random.NextFloat(0, 360), 0);
                EntityManager.SetComponentData(entity, currentObjectDefinition);
            }

            entities.Dispose();
        }
    }
}
