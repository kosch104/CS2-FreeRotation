using System.Reflection;
using Colossal.Entities;
using Colossal.Logging;
using Game;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
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
        public static Vector3 RotationDelta; // Stores delta of rotation which is to be applied next frame

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

        public void SetRotationDelta(float delta)
        {
            if (Mod.m_Setting.RotationAxis == RotationAxis.X)
            {
                RotationDelta = new Vector3(delta, 0, 0);
            }
            else if (Mod.m_Setting.RotationAxis == RotationAxis.Y)
            {
                RotationDelta = new Vector3(0, delta, 0);
            }
            else if (Mod.m_Setting.RotationAxis == RotationAxis.Z)
            {
                RotationDelta = new Vector3(0, 0, delta);
            }
        }

        public void OnFreelyRotateLeft(InputActionPhase phase)
        {
            m_Log.Info("Phase: " + phase);
        }

        public void OnFreelyRotateRight(InputActionPhase phase)
        {

        }

        public void OnDegreeRotateLeft(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Performed)
            {
                SetRotationDelta(-Mod.m_Setting.RotationDegrees);
                ForceUpdate();
            }
        }

        public void OnDegreeRotateRight(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Performed)
            {
                SetRotationDelta(Mod.m_Setting.RotationDegrees);
                ForceUpdate();
            }
        }


        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (RotationDelta == Vector3.zero)
            {
                return;
            }

            m_ObjectDefinitionQuery = SystemAPI.QueryBuilder()
                .WithAllRW<CreationDefinition, ObjectDefinition>()
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
                //var rotation = currentObjectDefinition.m_Rotation; //= Quaternion.Euler(RotationDelta);
                currentObjectDefinition.m_Rotation *= Quaternion.Euler(RotationDelta);
                RotationDelta = Vector3.zero;
                EntityManager.SetComponentData(entity, currentObjectDefinition);
                var field = typeof(ObjectToolSystem).GetField("m_RotationModified", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(m_ObjectToolSystem, true);
                }
            }

            entities.Dispose();
        }
    }
}
