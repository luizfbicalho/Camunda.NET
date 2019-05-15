/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.cmd
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using IdentityInfoEntity = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoEntity;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DeleteUserPictureCmd : Command<Void>
	{

	  protected internal string userId;

	  public DeleteUserPictureCmd(string userId)
	  {
		this.userId = userId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("UserId", userId);

		IdentityInfoEntity infoEntity = commandContext.IdentityInfoManager.findUserInfoByUserIdAndKey(userId, "picture");

		if (infoEntity != null)
		{
		  string byteArrayId = infoEntity.Value;
		  if (!string.ReferenceEquals(byteArrayId, null))
		  {
			commandContext.ByteArrayManager.deleteByteArrayById(byteArrayId);
		  }
		  commandContext.IdentityInfoManager.delete(infoEntity);
		}


		return null;
	  }

	}

}